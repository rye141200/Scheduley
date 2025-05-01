using System;

namespace Scheduley.Core.Options;

public class TwilioOptions
{
    public required string AccountSID { get; set; }
    public required string AuthToken { get; set; }
}
