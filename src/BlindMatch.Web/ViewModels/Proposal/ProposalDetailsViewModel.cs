using BlindMatch.Core.Enums;

namespace BlindMatch.Web.ViewModels.Proposal;

public class ProposalDetailsViewModel
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Abstract { get; set; } = string.Empty;
	public string TechnicalStack { get; set; } = string.Empty;
	public string Keywords { get; set; } = string.Empty;
	public string ResearchAreaName { get; set; } = string.Empty;
	public ProposalStatus Status { get; set; }
	public DateTime? SubmittedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public DateTime? WithdrawnAt { get; set; }
	public bool CanEdit { get; set; }
	public bool CanWithdraw { get; set; }
}
