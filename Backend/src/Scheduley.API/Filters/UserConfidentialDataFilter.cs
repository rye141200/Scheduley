using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Scheduley.API.Extensions;
using Scheduley.Core.Contracts;
using Scheduley.Core.Exceptions;

namespace Scheduley.API.Filters;

public class UserConfidentialDataFilter(
    ILogger<UserConfidentialDataFilter> logger,
    IUserCacheService userCacheService
// IGenericRepository<User> userRepository,
) : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext actionContext,
        ActionExecutionDelegate next
    )
    {
        var context = actionContext.HttpContext;
        Claim? emailClaim =
            context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
            ?? throw new AppError(
                "User not logged in!",
                System.Net.HttpStatusCode.Unauthorized,
                "Unauthorized",
                "Unauthorize"
            );

        string email = emailClaim.Value;

        var user =
            await userCacheService.GetUserAsync(email)
            ?? throw new AppError(
                "Authenticated user not found, maybe the user has been deleted after the token has been issued!",
                System.Net.HttpStatusCode.NotFound,
                "Not found",
                "User not found"
            );

        context.SetCurrentUser(user);

        logger.LogInformation("User ID populated successfully to the claims");

        await next();
    }
}
