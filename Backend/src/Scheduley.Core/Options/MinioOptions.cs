using System;

namespace Scheduley.Core.Options;

public class MinioOptions
{
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = "minioadmin";
    public string SecretKey { get; set; } = "minioadmin";
    public bool UseSSL { get; set; } = false;
    public string BucketName { get; set; } = "my-bucket";
    public int LinkExpiry { get; set; } = 60 * 60 * 24 * 7;
}
