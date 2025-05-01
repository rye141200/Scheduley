using System;
using System.ComponentModel.DataAnnotations;
using Scheduley.Core.Domain.Entities;

namespace Scheduley.Core.DTOs;

public abstract class MessageRequest
{
    [Required(ErrorMessage = "Message cannot be blank!")]
    public string? Content { get; set; }

    [Required(ErrorMessage = "Message must have a scheduled time!")]
    public DateTime ScheduledDate = DateTime.Now;

    public abstract Message ToMessage(Guid messageSenderID);
}
