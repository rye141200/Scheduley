using System;
using System.ComponentModel.DataAnnotations;
using Scheduley.Core.Domain.Entities;

namespace Scheduley.Core.DTOs;

public class WhatsAppMessageRequest : MessageRequest
{
    [Required]
    [Phone]
    public string RecipientPhoneNumber { get; set; } = string.Empty;

    public override Message ToMessage(Guid messageSenderID) =>
        new WhatsAppMessage()
        {
            Content = this.Content,
            RecipientPhoneNumber = this.RecipientPhoneNumber,
            ScheduledDate = this.ScheduledDate,
            UserId = messageSenderID,
        };
}
