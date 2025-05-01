using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Scheduley.API.Helpers;

public class ScheduleyJwtBearerEvents(IConfiguration config) : JwtBearerEvents
{
    public override Task MessageReceived(MessageReceivedContext context)
    {
        var accessTokenCookie = context.Request.Cookies[
            config["Authentication:Cookie:ScheduleyAccessToken:Name"]!
        ];

        context.Token = accessTokenCookie;
        return Task.CompletedTask;
    }
}
