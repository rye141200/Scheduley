using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Antiforgery;

namespace Scheduley.Core.Domain.Entities;

public class AppMessage : Message
{
    [Required]
    public Guid RecipientID { get; set; }
}
