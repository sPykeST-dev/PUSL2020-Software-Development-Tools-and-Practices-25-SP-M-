using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlindMatch.Web.ViewModels.Proposal;

public class CreateProposalViewModel
{
	[Required]
	[StringLength(150, MinimumLength = 5)]
	public string Title { get; set; } = string.Empty;

	[Required]
	[StringLength(2000, MinimumLength = 30)]
	public string Abstract { get; set; } = string.Empty;

	[Required]
	[Display(Name = "Technical Stack")]
	[StringLength(400)]
	public string TechnicalStack { get; set; } = string.Empty;

	[Required]
	[StringLength(250)]
	public string Keywords { get; set; } = string.Empty;

	[Required]
	[Display(Name = "Research Area")]
	public int? ResearchAreaId { get; set; }

	public List<SelectListItem> ResearchAreas { get; set; } = new();
}
