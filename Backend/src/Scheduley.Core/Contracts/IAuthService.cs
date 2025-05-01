using System;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Responses;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.DTOs;

namespace Scheduley.Core.Contracts;

public interface IAuthService
{
    Uri GetGoogleLoginURI();
    public Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string googleCallBackURI);
    public Task<GoogleJsonWebSignature.Payload?> GetPayload(TokenResponse tokenData);
}
