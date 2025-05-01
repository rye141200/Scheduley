using System;
using Microsoft.Extensions.Options;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Exceptions;
using Scheduley.Core.Helpers;
using Scheduley.Core.Options;

namespace Scheduley.API.Extensions;

public static class HttpContextExtensions
{
    private const string CurrentUserKey = "currentUser";

    private static CookieOptions GetAccessTokenCookieOptions(
        IOptions<CookieAuthOptions> cookieAuthOptions
    ) =>
        new()
        {
            Path = "/",
            HttpOnly = false,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = DateTime.UtcNow.AddMinutes(
                cookieAuthOptions.Value.ScheduleyAccessToken.ExpirationMinutes
            ),
        };

    private static CookieOptions GetRefreshTokenCookieOptions(
        IOptions<CookieAuthOptions> cookieAuthOptions
    ) =>
        new()
        {
            Path = "/",
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = DateTime.UtcNow.AddMinutes(
                cookieAuthOptions.Value.ScheduleyRefreshToken.ExpirationMinutes
            ),
        };

    public static void SetCurrentUser(this HttpContext context, User user)
    {
        context.Items[CurrentUserKey] = user;
    }

    private static User? GetCurrentUser(this HttpContext context)
    {
        return context.Items.TryGetValue(CurrentUserKey, out var value) ? value as User : null;
    }

    public static User GetRequiredCurrentUser(this HttpContext context) =>
        context.GetCurrentUser()
        ?? throw new AppError(
            "Current user does not exist, do not retrieve the user from context unless UserConfidentialDataFilter is applied!",
            System.Net.HttpStatusCode.BadRequest
        );

    public static bool IsSameSite(this HttpContext context)
    {
        var fetchSite = context.Request.Headers["Sec-Fetch-Site"].FirstOrDefault();
        return fetchSite == "same-origin" || fetchSite == "same-site";
    }

    public static void SetServerAuthCookie(
        this HttpContext context,
        string token,
        IOptions<CookieAuthOptions> cookieAuthOptions
    )
    {
        //!1) API server side cookie
        context.Response.Cookies.Append(
            cookieAuthOptions.Value.ScheduleyAuth.Name,
            token,
            new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(
                    cookieAuthOptions.Value.ScheduleyAuth.ExpirationMinutes
                ),
            }
        );
    }

    public static void SetAccessToken(
        this HttpContext context,
        string token,
        IOptions<CookieAuthOptions> cookieAuthOptions
    )
    {
        context.Response.Cookies.Append(
            cookieAuthOptions.Value.ScheduleyAccessToken.Name,
            token,
            GetAccessTokenCookieOptions(cookieAuthOptions)
        );
    }

    public static void SetRefreshToken(
        this HttpContext context,
        string token,
        IOptions<CookieAuthOptions> cookieAuthOptions
    )
    {
        context.Response.Cookies.Append(
            cookieAuthOptions.Value.ScheduleyRefreshToken.Name,
            token,
            GetRefreshTokenCookieOptions(cookieAuthOptions)
        );
    }

    public static void ClearRefreshAndAccessCookies(
        this HttpContext context,
        IOptions<CookieAuthOptions> cookieAuthOptions
    )
    {
        context.Response.Cookies.Delete(
            cookieAuthOptions.Value.ScheduleyAccessToken.Name,
            GetAccessTokenCookieOptions(cookieAuthOptions)
        );
        context.Response.Cookies.Delete(
            cookieAuthOptions.Value.ScheduleyRefreshToken.Name,
            GetRefreshTokenCookieOptions(cookieAuthOptions)
        );
    }
}
