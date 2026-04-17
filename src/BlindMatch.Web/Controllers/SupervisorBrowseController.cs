using System.Security.Claims;
using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.ValueObjects;
using BlindMatch.Web.ViewModels.Browse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlindMatch.Web.Controllers;

[Authorize(Roles = "Supervisor")]
public class SupervisorBrowseController : Controller
{
    private readonly IProposalRepository _proposalRepository;
    private readonly IRepository<ResearchArea> _researchAreaRepository;
    private readonly IInterestRepository _interestRepository;

    public SupervisorBrowseController(
        IProposalRepository proposalRepository,
        IRepository<ResearchArea> researchAreaRepository,
        IInterestRepository interestRepository)
    {
        _proposalRepository = proposalRepository;
        _researchAreaRepository = researchAreaRepository;
        _interestRepository = interestRepository;
    }

    public async Task<IActionResult> Index(int? researchAreaId, int page = 1, int pageSize = 10)
    {
        var allSubmitted = await _proposalRepository.GetAllSubmittedAsync();

        var filtered = researchAreaId.HasValue
            ? allSubmitted.Where(p => p.ResearchAreaId == researchAreaId.Value).ToList()
            : allSubmitted;

        var proposals = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var viewModel = new BrowseViewModel
        {
            Proposals = proposals.Select(MapToCardViewModel).ToList(),
            Filter = await GetFilterViewModel(researchAreaId),
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = filtered.Count,
                TotalPages = (int)Math.Ceiling((double)filtered.Count / pageSize)
            }
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var proposal = await _proposalRepository.GetByIdWithDetailsAsync(id);
        if (proposal == null || !proposal.SubmittedAt.HasValue)
            return NotFound();

        var supervisorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var alreadyExpressed = await _interestRepository.ExistsAsync(supervisorId, id);

        var viewModel = MapToDetailViewModel(proposal, !alreadyExpressed);
        return View(viewModel);
    }

    private AnonymisedProposalCardViewModel MapToCardViewModel(Proposal proposal)
    {
        return new AnonymisedProposalCardViewModel
        {
            Id = proposal.Id,
            ProjectCode = $"Project #{proposal.Id.ToString("D4")}",
            ResearchArea = proposal.ResearchArea.Name,
            Keywords = proposal.Keywords,
            SubmittedAt = proposal.SubmittedAt ?? proposal.UpdatedAt
        };
    }

    private AnonymisedProposalDetailViewModel MapToDetailViewModel(Proposal proposal, bool canExpressInterest)
    {
        return new AnonymisedProposalDetailViewModel
        {
            Id = proposal.Id,
            ProjectCode = $"Project #{proposal.Id.ToString("D4")}",
            Title = proposal.Title,
            Abstract = proposal.Abstract,
            TechnicalStack = proposal.TechnicalStack,
            Keywords = proposal.Keywords,
            ResearchArea = proposal.ResearchArea?.Name ?? "Unknown",
            SubmittedAt = proposal.SubmittedAt ?? proposal.UpdatedAt,
            CanExpressInterest = canExpressInterest
        };
    }

    private async Task<BrowseFilterViewModel> GetFilterViewModel(int? selectedResearchAreaId)
    {
        var researchAreas = await _researchAreaRepository.GetAllAsync();
        return new BrowseFilterViewModel
        {
            ResearchAreaId = selectedResearchAreaId,
            ResearchAreas = researchAreas
                .Where(ra => ra.IsActive)
                .Select(ra => new SelectListItem
                {
                    Value = ra.Id.ToString(),
                    Text = ra.Name,
                    Selected = ra.Id == selectedResearchAreaId
                })
                .ToList()
        };
    }
}

public class BrowseViewModel
{
    public List<AnonymisedProposalCardViewModel> Proposals { get; set; } = new();
    public BrowseFilterViewModel Filter { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();
}

public class PaginationViewModel
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
