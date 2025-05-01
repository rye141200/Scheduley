using System;
using System.Security.Claims;

namespace Scheduley.API.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string? GetUserClaimValue(this ClaimsPrincipal User, string claimType)
    {
        Claim? claim = User.Claims.FirstOrDefault(claim => claim.Type == claimType);
        if (claim == null)
            return null;
        return claim.Value;
    }
}
