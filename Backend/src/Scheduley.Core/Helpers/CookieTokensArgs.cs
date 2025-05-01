using System;

namespace Scheduley.Core.Helpers;

public class CookieTokensArgs
{
    public string? ServerCookieToken { get; set; }
    public string? AngularAccessToken { get; set; }
    public string? AngularRefreshToken { get; set; }
}
