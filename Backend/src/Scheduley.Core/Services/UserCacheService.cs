using System;
using Microsoft.Extensions.Caching.Memory;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Domain.RepositoryContracts;

namespace Scheduley.Core.Services;

public class UserCacheService(IMemoryCache memoryCache, IGenericRepository<User> userRepository)
    : IUserCacheService
{
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);
    private const string KeyPrefix = "User_";

    public async Task<User?> GetUserAsync(string email)
    {
        string cacheKey = $"{KeyPrefix}{email}";

        if (memoryCache.TryGetValue(cacheKey, out User? user))
            return user;

        user = await userRepository.FindOne(u => u.Email == email);

        if (user != null)
            memoryCache.Set(cacheKey, user, _cacheDuration);

        return user;
    }

    public Task SetUserAsync(User user)
    {
        if (user == null || string.IsNullOrEmpty(user.Email))
            return Task.CompletedTask;

        string cacheKey = $"{KeyPrefix}{user.Email}";
        memoryCache.Set(cacheKey, user, _cacheDuration);

        return Task.CompletedTask;
    }

    public Task RemoveUserAsync(string email)
    {
        string cacheKey = $"{KeyPrefix}{email}";
        memoryCache.Remove(cacheKey);

        return Task.CompletedTask;
    }

    public Task ClearCacheAsync()
    {
        if (memoryCache is MemoryCache cache)
            cache.Compact(1.0);

        return Task.CompletedTask;
    }
}
