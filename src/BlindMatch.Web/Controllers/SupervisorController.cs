using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlindMatch.Web.Controllers;

[Authorize(Roles = "Supervisor")]
public class SupervisorController : Controller
{
    private readonly ISupervisorRepository _supervisorRepository;
    private readonly IRepository<ResearchArea> _researchAreaRepository;
    private readonly SupervisorService _supervisorService;

    public SupervisorController(
        ISupervisorRepository supervisorRepository,
        IRepository<ResearchArea> researchAreaRepository,
        SupervisorService supervisorService)
    {
        _supervisorRepository = supervisorRepository;
        _researchAreaRepository = researchAreaRepository;
        _supervisorService = supervisorService;
    }

    public async Task<IActionResult> Profile()
    {
        var supervisorId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(supervisorId))
        {
            return Unauthorized();
        }

        var supervisor = await _supervisorRepository.GetByIdWithResearchAreasAsync(supervisorId);
        if (supervisor == null)
        {
            return NotFound();
        }

        var researchAreas = await _researchAreaRepository.GetAllAsync();
        var viewModel = new SupervisorProfileViewModel
        {
            Supervisor = supervisor,
            AvailableResearchAreas = researchAreas.Where(ra => ra.IsActive).ToList(),
            SelectedResearchAreaIds = supervisor.PreferredResearchAreas.Select(ra => ra.Id).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateResearchAreas(List<int> researchAreaIds)
    {
        var supervisorId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(supervisorId))
        {
            return Unauthorized();
        }

        var result = await _supervisorService.UpdateResearchAreasAsync(supervisorId, researchAreaIds);

        if (result.IsSuccess)
        {
            TempData["Success"] = "Research areas updated successfully.";
        }
        else
        {
            TempData["Error"] = result.Error;
        }

        return RedirectToAction("Profile");
    }
}

public class SupervisorProfileViewModel
{
    public Supervisor Supervisor { get; set; } = null!;
    public List<ResearchArea> AvailableResearchAreas { get; set; } = new();
    public List<int> SelectedResearchAreaIds { get; set; } = new();
}