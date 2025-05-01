using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Scheduley.API.Error;
using Scheduley.API.Filters;
using Scheduley.API.Helpers;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.RepositoryContracts;
using Scheduley.Core.Options;
using Scheduley.Core.Services;
using Scheduley.Infrastructure.DbContexts;
using Scheduley.Infrastructure.Repositories;
using Twilio;

namespace Scheduley.API.Extensions;

public static class AppServicesExtensions
{
    public static IServiceCollection RegisterCors(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddPolicy(
                "AllowAngular",
                builder =>
                    builder
                        .WithOrigins("http://localhost:4200")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
            )
        );
        return services;
    }

    public static IServiceCollection RegisterAuthentication(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        string? secretKey = config["Authentication:JWT:Key"];

        if (
            !int.TryParse(config["Authentication:JWT:ExpirationMinutes"], out int expirationMinutes)
        )
            expirationMinutes = 30;

        if (secretKey == null)
            throw new NullReferenceException("Secret key could not be found!");
        services.AddScoped<ScheduleyJwtBearerEvents>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey)
                        ),
                        ClockSkew = TimeSpan.Zero,
                    };

                    options.EventsType = typeof(ScheduleyJwtBearerEvents);
                }
            );

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection InitializeDatabase(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
        );
        return services;
    }

    public static IServiceCollection RegisterRepository(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        return services;
    }

    public static IServiceCollection RegisterServices(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        //!1) Custom services
        services.AddScoped(typeof(IMessageService<,>), typeof(MessageService<,>));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddMemoryCache();
        services.AddScoped<IUserCacheService, UserCacheService>();
        services.AddScoped<IUserService, UserService>();
        services.AddLogging();
        //!2) Scheduler
        //services.AddHostedService<MessageScheduleService>();

        //!3) Options
        services.Configure<JwtOptions>(config.GetSection("Authentication").GetSection("JWT"));
        services.Configure<GoogleAuthOptions>(
            config.GetSection("Authentication").GetSection("Google")
        );
        services.Configure<TwilioOptions>(config.GetSection("Twilio"));
        services.Configure<MinioOptions>(config.GetSection("Minio"));
        services.Configure<CookieAuthOptions>(
            config.GetSection("Authentication").GetSection("Cookie")
        );
        services.Configure<URLManagerOptions>(config.GetSection("URLManager"));
        //!4) MinIO

        services.AddSingleton<MinioClient>(options =>
        {
            var settings = options.GetRequiredService<IOptions<MinioOptions>>().Value;

            return (MinioClient)
                new MinioClient()
                    .WithEndpoint(settings.Endpoint)
                    .WithCredentials(settings.AccessKey, settings.SecretKey)
                    .WithSSL(settings.UseSSL)
                    .Build();
        });
        services.AddScoped<IMinioService, MinioService>();

        //!5) Filters
        services.AddScoped<UserConfidentialDataFilter>();
        services.AddScoped<AngularOnlyFilter>();

        //!6) Errors
        services.AddExceptionHandler<GlobalErrorHandler>();
        services.AddProblemDetails();

        //!7) Handlers
        services.AddScoped<CookieTokenHandler>();

        return services;
    }
}
