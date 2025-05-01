using System;
using System.ComponentModel.DataAnnotations;

namespace Scheduley.Core.Domain.Entities;

public class WhatsAppMessage : Message
{
    [Phone]
    [Required]
    public string? RecipientPhoneNumber { get; set; }
}
