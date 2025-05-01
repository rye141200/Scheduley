using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Options;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.ILM;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.Options;

namespace Scheduley.Core.Services;

public class MinioService(MinioClient minioClient, IOptions<MinioOptions> options) : IMinioService
{
    private readonly int BucketExpiryDays = 7;

    public async Task EnsureBucketExistsAsync(User user)
    {
        string bucketName = user.BucketName.ToString();
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        if (!await minioClient.BucketExistsAsync(bucketExistsArgs))
        {
            await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            await minioClient.SetBucketTagsAsync(
                new SetBucketTagsArgs()
                    .WithBucket(bucketName)
                    .WithTagging(
                        new Minio.DataModel.Tags.Tagging(
                            new Dictionary<string, string>() { ["email"] = user.Email },
                            false
                        )
                    )
            );
        }
    }

    public async Task<string> DownloadAsync(string objectName, User user)
    {
        string bucketName = user.BucketName.ToString();
        await EnsureBucketExistsAsync(user);
        PresignedGetObjectArgs args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithExpiry(options.Value.LinkExpiry);

        String url = await minioClient.PresignedGetObjectAsync(args);
        return url;
    }

    public async Task UploadAsync(string objectName, Stream data, long size, User user)
    {
        string bucketName = user.BucketName.ToString();
        await EnsureBucketExistsAsync(user);
        await minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(size)
        );
    }

    public List<Item> GetBucketItems(User user)
    {
        string bucketName = user.BucketName.ToString();
        var files = minioClient.ListObjectsEnumAsync(new ListObjectsArgs().WithBucket(bucketName));
        return [.. files.ToBlockingEnumerable()];
    }

    public async Task SetBucketLifetime(User user)
    {
        string bucketName = user.BucketName.ToString();
        await minioClient.SetBucketLifecycleAsync(
            new SetBucketLifecycleArgs()
                .WithBucket(bucketName)
                .WithLifecycleConfiguration(
                    new LifecycleConfiguration(
                        [
                            new()
                            {
                                ID = "",
                                Expiration = new Expiration() { Days = BucketExpiryDays },
                                Filter = new RuleFilter() { Prefix = "" },
                                Status = LifecycleRule.LifecycleRuleStatusEnabled,
                            },
                        ]
                    )
                )
        );
    }

    public ulong GetBucketSize(User user)
    {
        string bucketName = user.BucketName.ToString();
        var items = minioClient.ListObjectsEnumAsync(new ListObjectsArgs().WithBucket(bucketName));
        ulong size = 0;

        items.ToBlockingEnumerable().ToList().ForEach(item => size += item.Size);

        return size;
    }
}
