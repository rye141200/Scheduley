using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Scheduley.API.Filters;

public class AngularOnlyFilter : Attribute, IAuthorizationFilter
{
    private const string RequiredClientHeader = "AngularApp";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var headers = context.HttpContext.Request.Headers;

        if (
            !headers.TryGetValue("X-Client", out var clientHeader)
            || clientHeader != RequiredClientHeader
            || context.HttpContext.Request.Cookies["ScheduleyAuth"] != null
        )
            context.Result = new ForbidResult();
    }
}
