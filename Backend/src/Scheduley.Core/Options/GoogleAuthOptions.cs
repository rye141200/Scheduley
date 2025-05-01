using System;

namespace Scheduley.Core.Options;

public class GoogleAuthOptions
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string RedirectUriServer { get; set; }
    public required string RedirectUriAngular { get; set; }
}
