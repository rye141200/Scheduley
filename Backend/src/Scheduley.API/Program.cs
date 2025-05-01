using Scalar.AspNetCore;
using Scheduley.API.Error;
using Scheduley.API.Extensions;
using Scheduley.Core.Exceptions;

namespace Scheduley.API;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder
            .Services.InitializeDatabase(builder.Configuration)
            .RegisterRepository()
            .RegisterServices(builder.Configuration)
            .RegisterAuthentication(builder.Configuration)
            .RegisterCors();

        builder.WebHost.ConfigureKestrel(options =>
            options.Limits.MaxRequestBodySize = 100 * 1024 * 1024
        );

        builder.Services.AddOpenApi();

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("Scheduley API Documentation")
                .WithTheme(ScalarTheme.BluePlanet)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .WithOpenApiRoutePattern("/openapi/v1.json")
                .WithFavicon("/favicon.svg");
        });

        app.UseExceptionHandler();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("AllowAngular");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Map("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
        app.MapGet(
            "/buggy",
            () =>
            {
                throw new AppError(
                    "nigga",
                    System.Net.HttpStatusCode.BadRequest,
                    "Buggy endpoint",
                    "Congrats on reaching the buggy endpoint!"
                );
            }
        );

        app.Run();
    }
}
