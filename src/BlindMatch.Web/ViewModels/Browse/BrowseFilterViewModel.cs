using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlindMatch.Web.ViewModels.Browse;

public class BrowseFilterViewModel
{
    public int? ResearchAreaId { get; set; }
    public List<SelectListItem> ResearchAreas { get; set; } = new();
}
