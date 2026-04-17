using System.Security.Claims;
using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Services;
using BlindMatch.Web.ViewModels.Proposal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlindMatch.Web.Controllers;

[Authorize(Roles = "Student")]
public class ProposalController : Controller
{
	private readonly IProposalService _proposalService;

	public ProposalController(IProposalService proposalService)
	{
		_proposalService = proposalService;
	}

	public IActionResult Index() => RedirectToAction(nameof(Details));

	public async Task<IActionResult> Details()
	{
		var studentId = GetCurrentStudentId();
		if (studentId == null)
		{
			return Challenge();
		}

		var proposal = await _proposalService.GetStudentProposalAsync(studentId);
		if (proposal == null)
		{
			return RedirectToAction(nameof(Create));
		}

		var viewModel = new ProposalDetailsViewModel
		{
			Id = proposal.Id,
			Title = proposal.Title,
			Abstract = proposal.Abstract,
			TechnicalStack = proposal.TechnicalStack,
			Keywords = proposal.Keywords,
			ResearchAreaName = proposal.ResearchArea?.Name ?? "N/A",
			Status = proposal.Status,
			SubmittedAt = proposal.SubmittedAt,
			UpdatedAt = proposal.UpdatedAt,
			WithdrawnAt = proposal.WithdrawnAt,
			CanEdit = proposal.Status == ProposalStatus.Submitted,
			CanWithdraw = proposal.Status == ProposalStatus.Submitted
		};

		return View(viewModel);
	}

	[HttpGet]
	public async Task<IActionResult> Create()
	{
		var studentId = GetCurrentStudentId();
		if (studentId == null)
		{
			return Challenge();
		}

		var existingProposal = await _proposalService.GetStudentProposalAsync(studentId);
		if (existingProposal != null)
		{
			TempData["ErrorMessage"] = "You can only have one proposal.";
			return RedirectToAction(nameof(Details));
		}

		var viewModel = new CreateProposalViewModel();
		await PopulateResearchAreasAsync(viewModel);
		return View(viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(CreateProposalViewModel viewModel)
	{
		var studentId = GetCurrentStudentId();
		if (studentId == null)
		{
			return Challenge();
		}

		if (!ModelState.IsValid)
		{
			await PopulateResearchAreasAsync(viewModel);
			return View(viewModel);
		}

		var proposal = new Proposal
		{
			Title = viewModel.Title,
			Abstract = viewModel.Abstract,
			TechnicalStack = viewModel.TechnicalStack,
			Keywords = viewModel.Keywords,
			ResearchAreaId = viewModel.ResearchAreaId!.Value
		};

		var result = await _proposalService.CreateAsync(studentId, proposal);
		if (!result.IsSuccess)
		{
			ModelState.AddModelError(string.Empty, result.Error ?? "Failed to create proposal.");
			await PopulateResearchAreasAsync(viewModel);
			return View(viewModel);
		}

		TempData["SuccessMessage"] = "Proposal submitted successfully.";
		return RedirectToAction(nameof(Details));
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var studentId = GetCurrentStudentId();
		if (studentId == null)
		{
			return Challenge();
		}

		var proposal = await _proposalService.GetStudentProposalAsync(studentId);
		if (proposal == null || proposal.Id != id)
		{
			return NotFound();
		}

		if (proposal.Status != ProposalStatus.Submitted)
		{
			TempData["ErrorMessage"] = "This proposal can no longer be edited.";
			return RedirectToAction(nameof(Details));
		}

		var viewModel = new EditProposalViewModel
		{
			Id = proposal.Id,
			Title = proposal.Title,
			Abstract = proposal.Abstract,
			TechnicalStack = proposal.TechnicalStack,
			Keywords = proposal.Keywords,
			ResearchAreaId = proposal.ResearchAreaId,
			Status = proposal.Status
		};

		await PopulateResearchAreasAsync(viewModel);
		return View(viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(EditProposalViewModel viewModel)
	{
		var studentId = GetCurrentStudentId();
		if (studentId == null)
		{
			return Challenge();
		}

		if (!ModelState.IsValid)
		{
			await PopulateResearchAreasAsync(viewModel);
			return View(viewModel);
		}

		var proposal = new Proposal
		{
			Id = viewModel.Id,
			Title = viewModel.Title,
			Abstract = viewModel.Abstract,
			TechnicalStack = viewModel.TechnicalStack,
			Keywords = viewModel.Keywords,
			ResearchAreaId = viewModel.ResearchAreaId!.Value
		};

		var result = await _proposalService.UpdateAsync(studentId, proposal);
		if (!result.IsSuccess)
		{
			ModelState.AddModelError(string.Empty, result.Error ?? "Failed to update proposal.");
			await PopulateResearchAreasAsync(viewModel);
			return View(viewModel);
		}

		TempData["SuccessMessage"] = "Proposal updated successfully.";
		return RedirectToAction(nameof(Details));
	}

	[HttpGet]
	public async Task<IActionResult> Withdraw(int id)
	{
		var studentId = GetCurrentStudentId();
		if (studentId == null)
		{
			return Challenge();
		}

		var proposal = await _proposalService.GetStudentProposalAsync(studentId);
		if (proposal == null || proposal.Id != id)
		{
			return NotFound();
		}

		if (proposal.Status != ProposalStatus.Submitted)
		{
			TempData["ErrorMessage"] = "This proposal can no longer be withdrawn.";
			return RedirectToAction(nameof(Details));
		}

		var viewModel = new ProposalDetailsViewModel
		{
			Id = proposal.Id,
			Title = proposal.Title,
			Status = proposal.Status
		};

		return View(viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> WithdrawConfirmed(int id)
	{
		var studentId = GetCurrentStudentId();
		if (studentId == null)
		{
			return Challenge();
		}

		var result = await _proposalService.WithdrawAsync(studentId, id);
		if (!result.IsSuccess)
		{
			TempData["ErrorMessage"] = result.Error;
			return RedirectToAction(nameof(Details));
		}

		TempData["SuccessMessage"] = "Proposal withdrawn.";
		return RedirectToAction(nameof(Details));
	}

	private string? GetCurrentStudentId()
	{
		return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name;
	}

	private async Task PopulateResearchAreasAsync(CreateProposalViewModel viewModel)
	{
		var researchAreas = await _proposalService.GetActiveResearchAreasAsync();
		viewModel.ResearchAreas = researchAreas
			.OrderBy(ra => ra.Name)
			.Select(ra => new SelectListItem
			{
				Value = ra.Id.ToString(),
				Text = ra.Name,
				Selected = viewModel.ResearchAreaId == ra.Id
			})
			.ToList();
	}

	private async Task PopulateResearchAreasAsync(EditProposalViewModel viewModel)
	{
		var researchAreas = await _proposalService.GetActiveResearchAreasAsync();
		viewModel.ResearchAreas = researchAreas
			.OrderBy(ra => ra.Name)
			.Select(ra => new SelectListItem
			{
				Value = ra.Id.ToString(),
				Text = ra.Name,
				Selected = viewModel.ResearchAreaId == ra.Id
			})
			.ToList();
	}
}
