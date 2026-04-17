using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Web.ViewModels.ModuleLeader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlindMatch.Web.Pages.ModuleLeader;

[Authorize(Roles = "ModuleLeader")]
public class DashboardModel : PageModel
{
    private readonly IProposalRepository _proposalRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IAuditRepository _auditRepository;

    public DashboardModel(
        IProposalRepository proposalRepository,
        IMatchRepository matchRepository,
        IAuditRepository auditRepository)
    {
        _proposalRepository = proposalRepository;
        _matchRepository = matchRepository;
        _auditRepository = auditRepository;
    }

    public DashboardViewModel ViewModel { get; set; } = new();

    public async Task OnGetAsync()
    {
        var proposals = await _proposalRepository.GetAllAsync();
        ViewModel.TotalProposals = proposals.Count;

        ViewModel.ProposalsByStatus = proposals
            .GroupBy(p => p.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        ViewModel.PendingMatches = await _matchRepository.CountByStatusAsync(Core.Enums.MatchStatus.Pending);
        ViewModel.ActiveMatches = await _matchRepository.CountByStatusAsync(Core.Enums.MatchStatus.Approved);

        var recentAudit = await _auditRepository.GetRecentAsync(20);
        ViewModel.RecentActivity = recentAudit
            .Select(a => new DashboardViewModel.AuditActivityRow
            {
                When = a.Timestamp,
                Who = a.UserFullName ?? a.UserId ?? "System",
                Action = a.Action,
                Details = a.Details ?? ""
            })
            .ToList();
    }
}