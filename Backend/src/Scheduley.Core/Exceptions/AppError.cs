using System;
using System.Net;

namespace Scheduley.Core.Exceptions;

public class AppError : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorType { get; }
    public string Title { get; }

    public AppError(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        string errorType = "application_error",
        string title = "Application Error"
    )
        : base(message)
    {
        StatusCode = statusCode;
        ErrorType = errorType;
        Title = title;
    }
}
