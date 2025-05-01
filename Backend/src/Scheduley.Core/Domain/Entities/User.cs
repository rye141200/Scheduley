using System;
using System.ComponentModel.DataAnnotations;
using Scheduley.Core.Domain.Enums;

namespace Scheduley.Core.Domain.Entities;

public class User
{
    [Key]
    public Guid UserID { get; set; } = Guid.NewGuid();

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Name { get; set; }

    [Required]
    public Guid BucketName { get; set; } = Guid.NewGuid();

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? RefreshToken { get; set; }

    public DateTime LastLogin { get; set; } = DateTime.UtcNow;

    public Role UserRole { get; set; } = Role.Basic;
}
