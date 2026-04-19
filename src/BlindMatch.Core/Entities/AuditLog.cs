namespace BlindMatch.Core.Entities;

public class AuditLog
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public string? Details { get; set; }
}
