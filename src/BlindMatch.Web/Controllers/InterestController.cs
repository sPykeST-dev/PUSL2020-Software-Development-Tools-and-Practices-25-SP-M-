using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.ValueObjects;
using BlindMatch.Core.Common;
using BlindMatch.Infrastructure.Services;
using BlindMatch.Web.ViewModels.Interest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlindMatch.Web.Controllers;

[Authorize(Roles = "Supervisor")]
public class InterestController : Controller
{
    private readonly IInterestRepository _interestRepository;
    private readonly SupervisorService _supervisorService;
    private readonly IBlindMatchService _blindMatchService;

    public InterestController(
        IInterestRepository interestRepository,
        SupervisorService supervisorService,
        IBlindMatchService blindMatchService
)
    {
        _interestRepository = interestRepository;
        _supervisorService = supervisorService;
        _blindMatchService = blindMatchService;
    }

    public async Task<IActionResult> MyInterests()
    {
        var supervisorId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(supervisorId))
        {
            return Unauthorized();
        }

        var interests = await _interestRepository.GetInterestsBySupervisorAsync(supervisorId);
        var viewModel = interests.Select(MapToViewModel).ToList();

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ExpressInterest(int proposalId)
    {
        var supervisorId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(supervisorId))
        {
            return Unauthorized();
        }

        var result = await _supervisorService.ExpressInterestAsync(supervisorId, proposalId);

        if (result.IsSuccess)
        {
            TempData["Success"] = "Interest expressed successfully.";
            return RedirectToAction("Details", "SupervisorBrowse", new { id = proposalId });
        }
        else
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Details", "SupervisorBrowse", new { id = proposalId });
        }
    }

    public async Task<IActionResult> ConfirmInterest(int id)
    {
        var supervisorId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(supervisorId))
        {
            return Unauthorized();
        }

        var interest = await _interestRepository.GetByIdAsync(id);
        if (interest == null || interest.SupervisorId != supervisorId || interest.Status != Core.Enums.InterestStatus.Pending)
        {
            return NotFound();
        }

        var viewModel = new ConfirmInterestViewModel
        {
            InterestId = interest.Id,
            ProjectCode = $"Project #{interest.ProposalId.ToString("D4")}",
            Title = interest.Proposal.Title,
            SupervisorName = interest.Supervisor.FullName,
            SupervisorEmail = interest.Supervisor.Email ?? "",
            SupervisorDepartment = interest.Supervisor.Department
        };

        return View(viewModel);
    }

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Policy = Policies.SupervisorOnly)]
public async Task<IActionResult> ConfirmInterest(ConfirmInterestViewModel model)
{
    var supervisorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    var result = await _blindMatchService.ConfirmInterestAsync(model.InterestId, supervisorId);

    if (!result.IsSuccess)
    {
        ModelState.AddModelError(string.Empty, result.Error!);
        return View(model);
    }

    TempData["SuccessMessage"] = "Match confirmed. Identities have been revealed. You can now see the student's details below.";
    return RedirectToAction("MySupervisedMatches", "Match");
}

    private MyInterestItemViewModel MapToViewModel(SupervisorInterest interest)
    {
        return new MyInterestItemViewModel
        {
            Id = interest.Id,
            ProposalId = interest.ProposalId,
            ProjectCode = $"Project #{interest.ProposalId.ToString("D4")}",
            Title = interest.Proposal.Title,
            ResearchArea = interest.Proposal.ResearchArea.Name,
            Status = interest.Status,
            CreatedAt = interest.CreatedAt,
            CanConfirm = interest.Status == Core.Enums.InterestStatus.Pending
        };
    }
}
