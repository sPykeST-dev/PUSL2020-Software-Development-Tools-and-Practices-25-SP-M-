using BlindMatch.Core.Enums;

namespace BlindMatch.Web.ViewModels.Interest;

public class MyInterestItemViewModel
{
    public int Id { get; set; }
    public int ProposalId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ResearchArea { get; set; } = string.Empty;
    public InterestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool CanConfirm { get; set; }
}
