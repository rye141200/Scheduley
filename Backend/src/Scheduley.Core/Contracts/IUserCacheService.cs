using System;
using Scheduley.Core.Domain.Entities;

namespace Scheduley.Core.Contracts;

public interface IUserCacheService
{
    Task<User?> GetUserAsync(string email);
    Task SetUserAsync(User user);
    Task RemoveUserAsync(string email);
    Task ClearCacheAsync();
}
