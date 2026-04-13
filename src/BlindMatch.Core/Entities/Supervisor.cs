using System.ComponentModel.DataAnnotations;
using BlindMatch.Core.Entities;

namespace BlindMatch.Core.Entities;

public class Supervisor : ApplicationUser
{
    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public int MaxProjects { get; set; } = 3;

    [Required]
    public int CurrentProjects { get; set; } = 0;

    // Navigation properties
    public ICollection<SupervisorInterest> Interests { get; set; } = new List<SupervisorInterest>();
    public ICollection<ResearchArea> PreferredResearchAreas { get; set; } = new List<ResearchArea>();
}
