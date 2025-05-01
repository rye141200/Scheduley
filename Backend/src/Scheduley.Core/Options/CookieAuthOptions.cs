using System;

namespace Scheduley.Core.Options;

public class CookieAuthOptions
{
    public required CookieSettings ScheduleyAuth { get; set; }
    public required CookieSettings ScheduleyAccessToken { get; set; }
    public required CookieSettings ScheduleyRefreshToken { get; set; }
}

public class CookieSettings
{
    public string Name { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 5;
}
