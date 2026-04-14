using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.Interfaces.Services;
using BlindMatch.Core.ValueObjects;

namespace BlindMatch.Infrastructure.Services;

public class ProposalService : IProposalService
{
	private readonly IProposalRepository _proposalRepository;
	private readonly IRepository<ResearchArea> _researchAreaRepository;

	public ProposalService(
		IProposalRepository proposalRepository,
		IRepository<ResearchArea> researchAreaRepository)
	{
		_proposalRepository = proposalRepository;
		_researchAreaRepository = researchAreaRepository;
	}

	public async Task<Proposal?> GetStudentProposalAsync(string studentId)
	{
		return await _proposalRepository.GetByStudentIdAsync(studentId);
	}

	public async Task<ICollection<ResearchArea>> GetActiveResearchAreasAsync()
	{
		return await _researchAreaRepository.FindAsync(ra => ra.IsActive);
	}

	public async Task<Result> CreateAsync(string studentId, Proposal proposal)
	{
		if (string.IsNullOrWhiteSpace(studentId))
		{
			return Result.Failure("Student identity is required.");
		}

		if (await _proposalRepository.StudentHasProposalAsync(studentId))
		{
			return Result.Failure("Only one proposal is allowed per student.");
		}

		if (!await _researchAreaRepository.ExistsAsync(ra => ra.Id == proposal.ResearchAreaId && ra.IsActive))
		{
			return Result.Failure("Please select a valid research area.");
		}

		proposal.StudentId = studentId;
		proposal.Status = ProposalStatus.Submitted;
		proposal.SubmittedAt = DateTime.UtcNow;
		proposal.UpdatedAt = DateTime.UtcNow;
		proposal.WithdrawnAt = null;

		await _proposalRepository.AddAsync(proposal);
		return Result.Success();
	}

	public async Task<Result> UpdateAsync(string studentId, Proposal proposal)
	{
		var existingProposal = await _proposalRepository.GetByIdForStudentAsync(proposal.Id, studentId);
		if (existingProposal == null)
		{
			return Result.Failure("Proposal not found.");
		}

		if (existingProposal.Status == ProposalStatus.Matched)
		{
			return Result.Failure("You cannot edit a proposal after it has been matched.");
		}

		if (existingProposal.Status == ProposalStatus.Withdrawn)
		{
			return Result.Failure("You cannot edit a withdrawn proposal.");
		}

		if (!await _researchAreaRepository.ExistsAsync(ra => ra.Id == proposal.ResearchAreaId && ra.IsActive))
		{
			return Result.Failure("Please select a valid research area.");
		}

		existingProposal.Title = proposal.Title.Trim();
		existingProposal.Abstract = proposal.Abstract.Trim();
		existingProposal.TechnicalStack = proposal.TechnicalStack.Trim();
		existingProposal.Keywords = proposal.Keywords.Trim();
		existingProposal.ResearchAreaId = proposal.ResearchAreaId;
		existingProposal.UpdatedAt = DateTime.UtcNow;

		await _proposalRepository.UpdateAsync(existingProposal);
		return Result.Success();
	}

	public async Task<Result> WithdrawAsync(string studentId, int proposalId)
	{
		var existingProposal = await _proposalRepository.GetByIdForStudentAsync(proposalId, studentId);
		if (existingProposal == null)
		{
			return Result.Failure("Proposal not found.");
		}

		if (existingProposal.Status == ProposalStatus.Matched)
		{
			return Result.Failure("You cannot withdraw a proposal after it has been matched.");
		}

		if (existingProposal.Status == ProposalStatus.Withdrawn)
		{
			return Result.Failure("This proposal has already been withdrawn.");
		}

		existingProposal.Status = ProposalStatus.Withdrawn;
		existingProposal.WithdrawnAt = DateTime.UtcNow;
		existingProposal.UpdatedAt = DateTime.UtcNow;

		await _proposalRepository.UpdateAsync(existingProposal);
		return Result.Success();
	}
}
