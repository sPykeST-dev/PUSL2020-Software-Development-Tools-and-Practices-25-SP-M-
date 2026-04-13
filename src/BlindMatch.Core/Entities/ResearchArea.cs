// Stub implementation - will be completed by Member 6
namespace BlindMatch.Core.Entities;

public class ResearchArea
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
    public ICollection<Supervisor> Supervisors { get; set; } = new List<Supervisor>();
}
