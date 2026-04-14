using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Services;
using BlindMatch.Web.ViewModels.ModuleLeader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlindMatch.Web.Pages.ModuleLeader;

[Authorize(Roles = "ModuleLeader")]
public class ProposalOversightModel : PageModel
{
    private readonly IProposalRepository _proposalRepository;
    private readonly ModuleLeaderService _service;

    public ProposalOversightModel(
        IProposalRepository proposalRepository,
        ModuleLeaderService service)
    {
        _proposalRepository = proposalRepository;
        _service = service;
    }

    public List<ProposalOversightViewModel> Proposals { get; set; } = new();
    public ProposalStatus? FilterStatus { get; set; }

    public async Task OnGetAsync(ProposalStatus? filterStatus = null)
    {
        FilterStatus = filterStatus;
        var allProposals = await _proposalRepository.GetAllAsync();

        var filtered = filterStatus.HasValue
            ? allProposals.Where(p => p.Status == filterStatus.Value)
            : allProposals;

        Proposals = filtered
            .Select(p => new ProposalOversightViewModel
            {
                Id = p.Id,
                ProjectCode = $"#{p.Id:D4}",
                Title = p.Title,
                StudentName = p.Student?.FullName ?? "—",
                StudentEmail = p.Student?.Email ?? "—",
                ResearchArea = p.ResearchArea?.Name ?? "—",
                Status = p.Status ?? ProposalStatus.Draft,
                CreatedAt = p.CreatedAt,
                SubmittedAt = p.SubmittedAt
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(int proposalId, ProposalStatus newStatus)
    {
        var userId = User.FindFirst("UserId")?.Value;
        var fullName = User.Identity?.Name;

        var result = await _service.ChangeProposalStatusAsync(proposalId, newStatus, userId, fullName);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
        }
        else
        {
            TempData["Success"] = "Proposal status changed.";
        }

        return RedirectToPage();
    }
}