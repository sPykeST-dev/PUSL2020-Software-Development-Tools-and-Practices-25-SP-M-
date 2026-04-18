using System.ComponentModel.DataAnnotations;
using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;

namespace BlindMatch.Core.Entities;

public class SupervisorInterest
{
    public int Id { get; set; }

    [Required]
    public string SupervisorId { get; set; } = string.Empty;

    [Required]
    public int ProposalId { get; set; }

    [Required]
    public InterestStatus Status { get; set; } = InterestStatus.Pending;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ConfirmedAt { get; set; }

    public Supervisor Supervisor { get; set; } = null!;
    public Proposal Proposal { get; set; } = null!;
}
