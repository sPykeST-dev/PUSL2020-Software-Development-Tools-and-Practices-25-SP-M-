using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.ValueObjects;

namespace BlindMatch.Infrastructure.Services;

public class SupervisorService
{
    private readonly ISupervisorRepository _supervisorRepository;
    private readonly IInterestRepository _interestRepository;
    private readonly IProposalRepository _proposalRepository;

    public SupervisorService(
        ISupervisorRepository supervisorRepository,
        IInterestRepository interestRepository,
        IProposalRepository proposalRepository)
    {
        _supervisorRepository = supervisorRepository;
        _interestRepository = interestRepository;
        _proposalRepository = proposalRepository;
    }

    public async Task<Result> ExpressInterestAsync(string supervisorId, int proposalId)
    {
        // Check if interest already exists
        if (await _interestRepository.ExistsAsync(supervisorId, proposalId))
        {
            return Result.Failure("You have already expressed interest in this proposal.");
        }

        // Check supervisor capacity
        if (!await _supervisorRepository.HasCapacityAsync(supervisorId))
        {
            return Result.Failure("You have reached your maximum project capacity.");
        }

        // Check if proposal exists (basic check - Member 3 will implement proper validation)
        var proposal = await _proposalRepository.GetByIdAsync(proposalId);
        if (proposal == null)
        {
            return Result.Failure("Proposal not found.");
        }

        // Create interest
        var interest = new SupervisorInterest
        {
            SupervisorId = supervisorId,
            ProposalId = proposalId,
            Status = Core.Enums.InterestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _interestRepository.AddAsync(interest);

        return Result.Success();
    }

    public async Task<Result> UpdateResearchAreasAsync(string supervisorId, List<int> researchAreaIds)
    {
        // Simplified implementation - Member 1 will provide proper context access
        return Result.Failure("Research area update not implemented yet - waiting for Member 1");
    }
}