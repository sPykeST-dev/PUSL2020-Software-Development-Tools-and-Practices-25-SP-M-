namespace BlindMatch.Core.Entities;

public class AuditLog
{
    public int Id { get; set; }

    // The user who performed the action. Null for system events.
    public string? UserId { get; set; }

    // Short label, e.g. "ConfirmInterest" or "IdentityReveal".
    public string Action { get; set; } = string.Empty;

    // Entity type involved, e.g. "Match" or "Proposal".
    public string EntityName { get; set; } = string.Empty;

    // The entity's ID as a string.
    public string? EntityId { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    // extra context
    public string? Details { get; set; }
}