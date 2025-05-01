using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduley.Core.Contracts;
using Scheduley.Core.Domain.Entities;
using Scheduley.Core.DTOs;

namespace Scheduley.API.Controllers;

public class WhatsAppMessageController(
    IMessageService<WhatsAppMessage, WhatsAppMessageRequest> messageService
) : CustomControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllMessages() => Ok(await messageService.GetMessages());

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateMessage(WhatsAppMessageRequest messageRequest)
    {
        var idString = User
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (idString == null)
            return Unauthorized("Could not retrieve the user ID, please log in again");

        var message = await messageService.CreateMessage(Guid.Parse(idString), messageRequest);

        if (message == null)
            return BadRequest("Couldnt create the message try again later!");

        return Ok(message);
    }
}
