using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Minio.Exceptions;
using Scheduley.Core.Exceptions;

namespace Scheduley.API.Error;

public class GlobalErrorHandler(ILogger<GlobalErrorHandler> logger, IHostEnvironment env)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "An unhandled exception occurred");

        var problemDetails = MapExceptionToProblemDetails(exception, httpContext);

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        if (env.IsDevelopment())
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;

        httpContext.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private ProblemDetails MapExceptionToProblemDetails(
        Exception exception,
        HttpContext httpContext
    )
    {
        return exception switch
        {
            AppError appError => new ProblemDetails()
            {
                Status = (int)appError.StatusCode,
                Title = appError.Title,
                Detail = appError.Message,
                Type = appError.ErrorType,
                Instance = httpContext.Request.Path,
            },
            ArgumentException argEx => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = argEx.Message,
                Type = "bad_request",
                Instance = httpContext.Request.Path,
            },
            FileNotFoundException fileEx => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource Not Found",
                Detail = fileEx.Message,
                Type = "not_found",
                Instance = httpContext.Request.Path,
            },
            ObjectNotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Object Not Found",
                Detail = $"The requested object could not be found",
                Type = "object_not_found",
                Instance = httpContext.Request.Path,
            },
            BucketNotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Bucket Not Found",
                Detail = $"The requested bucket could not be found",
                Type = "bucket_not_found",
                Instance = httpContext.Request.Path,
            },
            MinioException minioEx => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Storage Service Error",
                Detail = minioEx.Message,
                Type = "storage_error",
                Instance = httpContext.Request.Path,
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = env.IsDevelopment() ? exception.Message : "An unexpected error occurred",
                Type = "internal_server_error",
                Instance = httpContext.Request.Path,
            },
        };
    }
}
