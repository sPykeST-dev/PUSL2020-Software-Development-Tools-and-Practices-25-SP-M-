namespace BlindMatch.Web.ViewModels.Browse;

public class AnonymisedProposalCardViewModel
{
    public int Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ResearchArea { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}
