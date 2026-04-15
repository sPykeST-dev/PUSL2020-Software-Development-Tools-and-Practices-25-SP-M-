namespace BlindMatch.Core.Entities;

public class IdentityReveal
{
    public int Id { get; set; }

    // One-to-one with Match.
    public int MatchId { get; set; }

    // UTC time identities were revealed.
    public DateTime RevealedAt { get; set; } = DateTime.UtcNow;

    // The supervisor who triggered the reveal by confirming.
    public string TriggeredBySupervisorId { get; set; } = string.Empty;

    // Navigation
    public Match Match { get; set; } = null!;
}