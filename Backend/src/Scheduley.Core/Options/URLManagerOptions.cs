using System;

namespace Scheduley.Core.Options;

public class URLManagerOptions
{
    public required string ServerBaseURL { get; set; }
    public required string AngularBaseURL { get; set; }
}
