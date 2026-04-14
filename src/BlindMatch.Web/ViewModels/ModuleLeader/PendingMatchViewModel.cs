using BlindMatch.Core.Enums;

namespace BlindMatch.Web.ViewModels.ModuleLeader;

public class PendingMatchViewModel
{
    public int Id { get; set; }
    public int ProposalId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProposalTitle { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string SupervisorName { get; set; } = string.Empty;
    public string SupervisorEmail { get; set; } = string.Empty;
    public string SupervisorDepartment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}