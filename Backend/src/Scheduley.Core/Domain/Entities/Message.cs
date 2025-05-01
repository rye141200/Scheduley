using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduley.Core.Domain.Entities;

public abstract class Message
{
    [Key]
    public Guid MessageID { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "A message cannot have blank content!")]
    [StringLength(100, ErrorMessage = "A message cannot be more than 100 characters")]
    public string? Content { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    [Required]
    public bool Sent { get; set; } = false;

    [ForeignKey("User")]
    public Guid UserId { get; set; }

    public virtual User? User { get; set; }
}
