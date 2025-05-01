using System;
using Minio.DataModel;
using Scheduley.Core.Domain.Entities;

namespace Scheduley.Core.Contracts;

public interface IMinioService
{
    Task EnsureBucketExistsAsync(User user);
    Task UploadAsync(string objectName, Stream data, long size, User user);
    Task<string> DownloadAsync(string objectName, User user);
    List<Item> GetBucketItems(User user);
    Task SetBucketLifetime(User user);
    ulong GetBucketSize(User user);
}
