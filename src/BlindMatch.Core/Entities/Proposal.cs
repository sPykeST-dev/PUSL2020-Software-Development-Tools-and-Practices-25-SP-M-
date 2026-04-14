using System.ComponentModel.DataAnnotations;
using BlindMatch.Core.Enums;

namespace BlindMatch.Core.Entities;

public class Proposal
{
    public int Id { get; set; }

    [Required]
    [StringLength(150, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 30)]
    public string Abstract { get; set; } = string.Empty;

    [Required]
    [StringLength(400)]
    public string TechnicalStack { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Keywords { get; set; } = string.Empty;

    [Required]
    public int ResearchAreaId { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    public ProposalStatus Status { get; set; } = ProposalStatus.Submitted;

    public DateTime? SubmittedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? WithdrawnAt { get; set; }

    // Navigation properties
    public ResearchArea ResearchArea { get; set; } = null!;
    public Student Student { get; set; } = null!;
    public ICollection<SupervisorInterest> Interests { get; set; } = new List<SupervisorInterest>();
}
