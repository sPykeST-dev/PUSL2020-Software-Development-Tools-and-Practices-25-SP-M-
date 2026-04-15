using BlindMatch.Core.Enums;

namespace BlindMatch.Web.ViewModels.Match;

public class SupervisedMatchItemViewModel
{
    public int MatchId { get; set; }
    public string ProposalTitle { get; set; } = string.Empty;
    public MatchStatus MatchStatus { get; set; }
    public string StudentFullName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string? Programme { get; set; }
    public int? YearOfStudy { get; set; }
}