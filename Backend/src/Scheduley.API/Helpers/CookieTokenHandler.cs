using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Scheduley.API.Extensions;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Domain.RepositoryContracts;
using Scheduley.Core.DTOs;
using Scheduley.Core.Options;

namespace Scheduley.API.Helpers;

public class CookieTokenHandler(
    IOptions<CookieAuthOptions> cookieOptions,
    IJwtTokenService tokenService,
    IUserCacheService userCacheService,
    IOptions<URLManagerOptions> urlManager,
    IUserService userService
)
{
    public async Task<IActionResult> AngularCookieTokenHandler(User user, HttpContext context)
    {
        //!1) Generate token(s)
        var expirationMinutes = cookieOptions.Value.ScheduleyAccessToken.ExpirationMinutes;
        var accessToken = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        //!2) Set cookie(s)
        context.SetAccessToken(accessToken, cookieOptions);
        context.SetRefreshToken(refreshToken.ToString(), cookieOptions);

        //!3) Store refresh token
        await userService.UpdateUserRefreshTokenAsync(user, refreshToken);

        //!4) Cache user
        await userCacheService.SetUserAsync(user);

        return new RedirectResult(urlManager.Value.AngularBaseURL);
    }

    public async Task<IActionResult> ServerCookieTokenHandler(User user, HttpContext context)
    {
        //!1) Generate token(s)
        var expirationMinutes = cookieOptions.Value.ScheduleyAccessToken.ExpirationMinutes;
        var accessToken = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        //!2) Set cookie(s)
        context.SetAccessToken(accessToken, cookieOptions);
        context.SetRefreshToken(refreshToken.ToString(), cookieOptions);

        //!3) Store refresh token
        await userService.UpdateUserRefreshTokenAsync(user, refreshToken);

        //!4) Cache
        await userCacheService.SetUserAsync(user);

        return new RedirectResult($"{urlManager.Value.ServerBaseURL}scalar");
    }
}
