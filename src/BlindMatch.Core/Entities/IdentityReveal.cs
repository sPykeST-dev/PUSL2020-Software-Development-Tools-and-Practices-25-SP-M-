using System.ComponentModel.DataAnnotations;

namespace BlindMatch.Core.Entities;

public class IdentityReveal
{
    public int Id { get; set; }

    [Required]
    public int MatchId { get; set; }

    [Required]
    public DateTime RevealedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Match Match { get; set; } = null!;
}