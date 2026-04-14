using BlindMatch.Core.Enums;

namespace BlindMatch.Web.ViewModels.ModuleLeader;

public class ProposalOversightViewModel
{
    public int Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string ResearchArea { get; set; } = string.Empty;
    public ProposalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}