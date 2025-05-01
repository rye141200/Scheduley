using System;
using Google.Apis.Auth;
using Scheduley.Core.Domain.Entities;

namespace Scheduley.Core.Contracts;

public interface IUserService
{
    Task<User> FindOrCreateUserAsync(GoogleJsonWebSignature.Payload payload);
    Task UpdateUserRefreshTokenAsync(User user, Guid newRefreshToken);
    Task<User?> GetUserByEmail(string email);
    Task UpdateUser(User user, Action<User> updateDelegate);
    Task ClearUserRefreshToken(User user);
}
