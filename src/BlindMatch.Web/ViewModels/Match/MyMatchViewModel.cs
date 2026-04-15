using BlindMatch.Core.Enums;

namespace BlindMatch.Web.ViewModels.Match;

public class MyMatchViewModel
{
    public int ProposalId { get; set; }
    public MatchStatus MatchStatus { get; set; }
    public string SupervisorFullName { get; set; } = string.Empty;
    public string SupervisorEmail { get; set; } = string.Empty;
    public string SupervisorDepartment { get; set; } = string.Empty;
}