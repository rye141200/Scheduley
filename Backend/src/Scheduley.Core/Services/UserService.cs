using System;
using Google.Apis.Auth;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Domain.RepositoryContracts;
using Scheduley.Core.Exceptions;

namespace Scheduley.Core.Services;

public class UserService(IGenericRepository<User> userRepository, IMinioService minioService)
    : IUserService
{
    public async Task<User> FindOrCreateUserAsync(GoogleJsonWebSignature.Payload payload)
    {
        string userEmail = payload.Email;
        string userName = payload.Name;

        var user = await userRepository.FindOne(user => user.Email == userEmail);
        if (user == null)
        {
            user = await userRepository.CreateAndGet(
                new User()
                {
                    Email = userEmail,
                    Name = userName,
                    LastLogin = DateTime.UtcNow,
                }
            );
            await minioService.EnsureBucketExistsAsync(user);
        }
        else
        {
            user.LastLogin = DateTime.UtcNow;
            userRepository.Update(user);
        }

        await userRepository.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserRefreshTokenAsync(User user, Guid newRefreshToken)
    {
        user.RefreshToken = newRefreshToken;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
    }

    public async Task<User?> GetUserByEmail(string email) =>
        await userRepository.FindOne(user => user.Email == email);

    public async Task ClearUserRefreshToken(User user)
    {
        user.RefreshToken = null;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
    }

    public async Task UpdateUser(User user, Action<User> updateDelegate)
    {
        updateDelegate(user);
        await userRepository.SaveChangesAsync();
        /* var userType = typeof(User);
        userType.GetProperty("Name"); */
    }
}
