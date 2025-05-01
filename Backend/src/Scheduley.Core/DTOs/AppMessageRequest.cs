using System;
using Scheduley.Core.Domain.Entities;

namespace Scheduley.Core.DTOs;

public class AppMessageRequest : MessageRequest
{
    public Guid RecipientID { get; set; }

    public override Message ToMessage(Guid messageSenderID)
    {
        return new AppMessage()
        {
            Content = this.Content,
            ScheduledDate = this.ScheduledDate,
            UserId = messageSenderID,
            RecipientID = RecipientID,
        };
    }
}
