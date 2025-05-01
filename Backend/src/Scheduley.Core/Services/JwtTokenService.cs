using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Exceptions;
using Scheduley.Core.Helpers;
using Scheduley.Core.Options;

namespace Scheduley.Core.Services;

public class JwtTokenService(IOptions<JwtOptions> jwtOptions, ILogger<JwtTokenService> logger)
    : IJwtTokenService
{
    private ClaimsPrincipal ExtractClaimsPrincipal(string token)
    {
        var securityTokenHandler = new JwtSecurityTokenHandler();

        if (!securityTokenHandler.CanReadToken(token))
            throw new AppError(
                "Token has been malformed or not valid JWT!",
                System.Net.HttpStatusCode.Unauthorized
            );
        return securityTokenHandler.ValidateToken(
            token,
            new()
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(jwtOptions.Value.Key)
                ),
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuer = false,
            },
            out _
        );
    }

    public string ExtractUserEmail(string token)
    {
        var emailClaim =
            ExtractClaimsPrincipal(token)
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
            ?? throw new AppError("Email not found!", System.Net.HttpStatusCode.NotFound);
        return emailClaim.Value;
    }

    public Guid GenerateRefreshToken() => Guid.NewGuid();

    public bool IsValidRefreshToken(User user, Guid refreshToken) =>
        user.RefreshToken == refreshToken;

    public string GenerateToken(User user)
    {
        //!1) Key
        var key = Encoding.ASCII.GetBytes(jwtOptions.Value.Key);
        if (key.Length < 64)
            throw new AppError(
                "Key length must be at least 64 characters!",
                System.Net.HttpStatusCode.BadRequest,
                "Key error",
                "Key length was not set correctly"
            );

        //!2) Claims
        var claims = new List<Claim>
        {
            /* new(ClaimTypes.NameIdentifier, user.UserID.ToString()), */
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name ?? ""),
            new(ClaimTypes.Role, user.UserRole.ToString()),
            new(CustomClaims.CreatedAt, user.CreatedAt.ToShortDateString()),
            new(CustomClaims.LastLogin, user.LastLogin.ToLongTimeString()),
        };

        //!3) Token descriptor
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
        {
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpirationMinutes),
        };

        //!4) Token handler
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public bool IsExpiredToken(string token)
    {
        var securityTokenHandler = new JwtSecurityTokenHandler();
        if (!securityTokenHandler.CanReadToken(token))
            throw new AppError(
                "Token has been malformed or not valid JWT!",
                System.Net.HttpStatusCode.Unauthorized
            );
        var securityToken = securityTokenHandler.ReadJwtToken(token);
        return securityToken.ValidTo < DateTime.UtcNow;
    }

    public bool isValidToken(string token)
    {
        var securityTokenHandler = new JwtSecurityTokenHandler();
        if (!securityTokenHandler.CanReadToken(token))
            throw new AppError(
                "Token has been malformed or not valid JWT!",
                System.Net.HttpStatusCode.Unauthorized
            );
        try
        {
            var principal = securityTokenHandler.ValidateToken(
                token,
                new()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(jwtOptions.Value.Key)
                    ),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuer = false,
                },
                out _
            );
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex.StackTrace);
            return false;
        }
    }
}
