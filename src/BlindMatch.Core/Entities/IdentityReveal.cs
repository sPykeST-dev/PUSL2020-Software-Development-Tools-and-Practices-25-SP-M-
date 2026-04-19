namespace BlindMatch.Core.Entities;

public class IdentityReveal
{
    public int Id { get; set; }

    public int MatchId { get; set; }

    public DateTime RevealedAt { get; set; } = DateTime.UtcNow;

    public string TriggeredBySupervisorId { get; set; } = string.Empty;

    public Match Match { get; set; } = null!;
}
