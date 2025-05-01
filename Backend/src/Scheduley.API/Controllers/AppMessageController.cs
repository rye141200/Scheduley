using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduley.API.Extensions;
using Scheduley.API.Filters;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.DTOs;

namespace Scheduley.API.Controllers;

public class AppMessageController(
    IMessageService<AppMessage, AppMessageRequest> messageService
// ILogger<AppMessageController> logger
) : CustomControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllMessages() => Ok(await messageService.GetMessages());

    [HttpPost]
    [Authorize]
    [ServiceFilter(typeof(UserConfidentialDataFilter))]
    public async Task<IActionResult> CreateMessage(AppMessageRequest messageRequest)
    {
        //! Implement the logic where we retrieve the authenticated user ID and then we create the message
        var user = HttpContext.GetRequiredCurrentUser();

        if (user == null)
            return Unauthorized("Could not retrieve the user ID, please log in again");

        var message = await messageService.CreateMessage(user.UserID, messageRequest);

        if (message == null)
            return BadRequest("Couldnt create the message try again later!");

        return Ok(message);
    }
}
