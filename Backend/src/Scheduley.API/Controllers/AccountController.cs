using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Scheduley.API.Extensions;
using Scheduley.API.Filters;
using Scheduley.API.Helpers;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Exceptions;
using Scheduley.Core.Options;

namespace Scheduley.API.Controllers;

[Tags("Authentication")]
public class AccountController(
    IAuthService authService,
    IOptions<GoogleAuthOptions> googleOptions,
    IOptions<CookieAuthOptions> cookieOptions,
    CookieTokenHandler cookieTokenHandler,
    IJwtTokenService jwtTokenService,
    IUserCacheService userCacheService,
    IUserService userService
) : CustomControllerBase
{
    [HttpGet("google")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult GoogleLogin() => Redirect(authService.GetGoogleLoginURI().ToString());

    [HttpGet("google-login-scalar")]
    public IActionResult GoogleLoginScalar() => Ok(authService.GetGoogleLoginURI().ToString());

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallbackServer([FromQuery] string code) =>
        await ProcessGoogleLogin(
            code,
            googleOptions.Value.RedirectUriServer,
            cookieTokenHandler.ServerCookieTokenHandler
        );

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("google-callback-angular")]
    public async Task<IActionResult> AngularGoogleCallback([FromQuery] string code) =>
        await ProcessGoogleLogin(
            code,
            googleOptions.Value.RedirectUriAngular,
            cookieTokenHandler.AngularCookieTokenHandler
        );

    #region Annotations
    [HttpGet]
    [Authorize]
    [ServiceFilter(typeof(UserConfidentialDataFilter))]
    [EndpointSummary("Get user data")]
    [EndpointDescription(
        "Protected endpoint that validates the user's authentication token and returns user claims"
    )]
    [EndpointName("ValidateToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    #endregion
    public IActionResult ProtectedDummyRoute()
    {
        var user = HttpContext.GetRequiredCurrentUser();
        if (user == null)
            return NotFound("User not found!");
        return Ok(
            User.Claims.Select(claim =>
                {
                    var claimName = claim.Type.Split("/")[^1];
                    return new
                    {
                        type = claim.Type.Split("/")[^1],
                        value = claimName == "nbf" || claimName == "exp" || claimName == "iat"
                            ? DateTimeOffset
                                .FromUnixTimeSeconds(long.Parse(claim.Value))
                                .ToLocalTime()
                                .LocalDateTime.ToString()
                            : claim.Value,
                    };
                })
                .ToList()
        );
    }

    [HttpGet("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        //!1) Extract tokens
        var accessToken = HttpContext.Request.Cookies[
            cookieOptions.Value.ScheduleyAccessToken.Name
        ];

        bool refreshTokenExists = Guid.TryParse(
            HttpContext.Request.Cookies[cookieOptions.Value.ScheduleyRefreshToken.Name],
            out var refreshToken
        );

        //!2) Unauthorized access
        if (
            accessToken == null
            || !refreshTokenExists
            || !jwtTokenService.isValidToken(accessToken)
        )
            return Unauthorized("You shall not pass!");

        //!3) Authorized and no need to refresh
        if (!jwtTokenService.IsExpiredToken(accessToken))
            return Ok("No need to refresh");

        //!4) Extract user from token
        string userEmail = jwtTokenService.ExtractUserEmail(accessToken);
        var user =
            await userService.GetUserByEmail(userEmail)
            ?? throw new AppError(
                "Unauthorized, user does not exist or may have been deleted after issuing the token",
                System.Net.HttpStatusCode.Unauthorized
            );

        if (!jwtTokenService.IsValidRefreshToken(user, refreshToken))
            return Unauthorized("Refresh token is invalid!");

        //!5) Update refresh token
        await userService.UpdateUserRefreshTokenAsync(user, jwtTokenService.GenerateRefreshToken());

        //!6) Set new tokens in cookies
        await SetAccessAndRefreshTokensCookiesAsync(user);

        return Ok(new { message = "Refreshed successfully!" });
    }

    [HttpGet("logout")]
    [Authorize]
    [ServiceFilter(typeof(UserConfidentialDataFilter))]
    public async Task<IActionResult> Logout()
    {
        HttpContext.ClearRefreshAndAccessCookies(cookieOptions);

        var user = HttpContext.GetRequiredCurrentUser();
        await userService.ClearUserRefreshToken(user);
        await userCacheService.RemoveUserAsync(user.Email);

        return Ok(new { message = "Logged out successfully!" });
    }

    [HttpGet("force-logout")]
    public IActionResult ForceLogout()
    {
        HttpContext.ClearRefreshAndAccessCookies(cookieOptions);
        return Ok();
    }

    private async Task SetAccessAndRefreshTokensCookiesAsync(User user)
    {
        if (user.RefreshToken == null || user.RefreshToken == Guid.Empty)
            throw new AppError(
                "Refresh token is invalid or null!",
                System.Net.HttpStatusCode.BadRequest
            );

        //!1) Reset cookies
        HttpContext.SetRefreshToken(user.RefreshToken.ToString()!, cookieOptions);
        HttpContext.SetAccessToken(jwtTokenService.GenerateToken(user), cookieOptions);

        //!2) Cache user
        await userCacheService.SetUserAsync(user);
    }

    //? Method name should be ProccessLogin only
    private async Task<IActionResult> ProcessGoogleLogin(
        string code,
        string googleCallBackURI,
        Func<User, HttpContext, Task<IActionResult>> cookieTokenHandler
    )
    {
        //? The first two steps should also be encapsulated into handlers that depends on the auth provider (Google, Facebook etc)
        //! 1. Validate Google authentication
        var tokenData =
            await authService.ExchangeCodeForTokenAsync(code, googleCallBackURI)
            ?? throw new AppError("Invalid google token!", System.Net.HttpStatusCode.Unauthorized);

        var payload =
            await authService.GetPayload(tokenData)
            ?? throw new AppError("Email is not verified!", System.Net.HttpStatusCode.Unauthorized);

        //! 2. Find or create user
        var user = await userService.FindOrCreateUserAsync(payload);

        //? Delegation depends on who the audience are (Angular, Scalar API Docs, MVC or Android app etc)
        //! 3. Delegate cookie and token creation to handlers
        return await cookieTokenHandler(user, HttpContext);
    }
}
