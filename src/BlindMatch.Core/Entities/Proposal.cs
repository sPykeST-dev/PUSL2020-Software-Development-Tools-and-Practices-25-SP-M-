// Stub implementation - will be completed by Member 3
namespace BlindMatch.Core.Entities;

public class Proposal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string TechnicalStack { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public int ResearchAreaId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }

    // Navigation properties
    public ResearchArea ResearchArea { get; set; } = null!;
    public ICollection<SupervisorInterest> Interests { get; set; } = new List<SupervisorInterest>();
}
