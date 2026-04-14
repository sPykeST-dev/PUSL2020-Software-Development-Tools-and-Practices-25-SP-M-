using System.ComponentModel.DataAnnotations;

namespace BlindMatch.Core.Entities;

public class AuditEvent
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [StringLength(450)]
    public string? UserId { get; set; }

    [StringLength(200)]
    public string? UserFullName { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Details { get; set; }

    // Navigation property
    public ApplicationUser? User { get; set; }
}