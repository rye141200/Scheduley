using System;
using System.Net.Http.Json;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Domain.RepositoryContracts;
using Scheduley.Core.DTOs;
using Scheduley.Core.Exceptions;
using Scheduley.Core.Options;

namespace Scheduley.Core.Services;

public class AuthService(IOptions<GoogleAuthOptions> options) : IAuthService
{
    public Uri GetGoogleLoginURI() =>
        new(
            $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={options.Value.ClientId}&redirect_uri={options.Value.RedirectUriServer}&scope=openid email profile&state={Guid.NewGuid()}"
        );

    public async Task<TokenResponse> ExchangeCodeForTokenAsync(
        string code,
        string googleCallBackURI
    )
    {
        var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = options.Value.ClientId,
                    ClientSecret = options.Value.ClientSecret,
                },
                Scopes = ["openid", "profile", "email"],
            }
        );

        var tokens =
            await flow.ExchangeCodeForTokenAsync(
                userId: null,
                code,
                googleCallBackURI,
                CancellationToken.None
            )
            ?? throw new AppError(
                "Authentication error, cannot exchange authorization code with google",
                System.Net.HttpStatusCode.Unauthorized
            );

        return tokens;
    }

    public async Task<GoogleJsonWebSignature.Payload?> GetPayload(TokenResponse tokenData)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(
            tokenData.IdToken,
            new GoogleJsonWebSignature.ValidationSettings { Audience = [options.Value.ClientId] }
        );

        if (payload == null || !payload.EmailVerified)
            return null;
        return payload;
    }
}
