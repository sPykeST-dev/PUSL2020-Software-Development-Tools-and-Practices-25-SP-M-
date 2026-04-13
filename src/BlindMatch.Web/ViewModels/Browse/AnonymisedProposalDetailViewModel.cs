namespace BlindMatch.Web.ViewModels.Browse;

public class AnonymisedProposalDetailViewModel
{
    public int Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string TechnicalStack { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string ResearchArea { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public bool CanExpressInterest { get; set; }
}
