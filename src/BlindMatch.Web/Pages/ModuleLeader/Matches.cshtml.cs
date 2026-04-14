using BlindMatch.Core.Common;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Services;
using BlindMatch.Web.ViewModels.ModuleLeader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlindMatch.Web.Pages.ModuleLeader;

[Authorize(Roles = "ModuleLeader")]
public class MatchesModel : PageModel
{
    private readonly IMatchRepository _matchRepository;
    private readonly ISupervisorRepository _supervisorRepository;
    private readonly ModuleLeaderService _service;

    public MatchesModel(
        IMatchRepository matchRepository,
        ISupervisorRepository supervisorRepository,
        ModuleLeaderService service)
    {
        _matchRepository = matchRepository;
        _supervisorRepository = supervisorRepository;
        _service = service;
    }

    public List<PendingMatchViewModel> PendingMatches { get; set; } = new();
    public List<Supervisor> AllSupervisors { get; set; } = new();

    public async Task OnGetAsync()
    {
        var matches = await _matchRepository.GetByStatusAsync(MatchStatus.Pending);
        PendingMatches = matches
            .Select(m => new PendingMatchViewModel
            {
                Id = m.Id,
                ProposalId = m.ProposalId,
                ProjectCode = $"#{m.ProposalId:D4}",
                ProposalTitle = m.Proposal?.Title ?? "—",
                StudentName = m.Student?.FullName ?? "—",
                StudentEmail = m.Student?.Email ?? "—",
                SupervisorName = m.Supervisor?.FullName ?? "—",
                SupervisorEmail = m.Supervisor?.Email ?? "—",
                SupervisorDepartment = m.Supervisor?.Department ?? "—",
                CreatedAt = m.CreatedAt
            })
            .ToList();

        AllSupervisors = (await _supervisorRepository.GetAllAsync()).ToList();
    }

    public async Task<IActionResult> OnPostApproveAsync(int matchId)
    {
        var userId = User.FindFirst("UserId")?.Value;
        var fullName = User.Identity?.Name;

        var result = await _service.ApproveMatchAsync(matchId, userId, fullName);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
        }
        else
        {
            TempData["Success"] = "Match approved.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int matchId, [FromForm] string rejectReason)
    {
        if (string.IsNullOrWhiteSpace(rejectReason))
        {
            TempData["Error"] = "Rejection reason is required.";
            return RedirectToPage();
        }

        var userId = User.FindFirst("UserId")?.Value;
        var fullName = User.Identity?.Name;

        var result = await _service.RejectMatchAsync(matchId, rejectReason, userId, fullName);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
        }
        else
        {
            TempData["Success"] = "Match rejected. Proposal returned to pool.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReassignAsync(int matchId, [FromForm] string newSupervisorId)
    {
        if (string.IsNullOrWhiteSpace(newSupervisorId))
        {
            TempData["Error"] = "Supervisor selection is required.";
            return RedirectToPage();
        }

        var userId = User.FindFirst("UserId")?.Value;
        var fullName = User.Identity?.Name;

        var result = await _service.ReassignMatchAsync(matchId, newSupervisorId, userId, fullName);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
        }
        else
        {
            TempData["Success"] = "Match reassigned.";
        }

        return RedirectToPage();
    }
}