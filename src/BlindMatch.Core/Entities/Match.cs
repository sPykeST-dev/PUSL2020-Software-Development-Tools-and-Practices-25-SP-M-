using BlindMatch.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace BlindMatch.Core.Entities;

public class Match
{
    public int Id { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    public string SupervisorId { get; set; } = string.Empty;

    [Required]
    public int ProposalId { get; set; }

    [Required]
    public MatchStatus Status { get; set; } = MatchStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Module Leader approval dashboard
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }

    [StringLength(500)]
    public string RejectionReason { get; set; } = string.Empty;

    // Navigation properties
    public Proposal Proposal { get; set; } = null!;
    public ApplicationUser Student { get; set; } = null!;
    public Supervisor Supervisor { get; set; } = null!;
    public IdentityReveal? IdentityReveal { get; set; }
}