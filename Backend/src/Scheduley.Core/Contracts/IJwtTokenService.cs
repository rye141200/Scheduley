using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Exceptions;

namespace Scheduley.Core.Contracts;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    bool IsExpiredToken(string token);
    Guid GenerateRefreshToken();
    bool IsValidRefreshToken(User user, Guid refreshToken);
    string ExtractUserEmail(string token);
    public bool isValidToken(string token);
}
