using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.ValueObjects;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Infrastructure.Services;

public class SupervisorService
{
    private readonly ISupervisorRepository _supervisorRepository;
    private readonly IInterestRepository _interestRepository;
    private readonly IProposalRepository _proposalRepository;
    private readonly ApplicationDbContext _context;

    public SupervisorService(
        ISupervisorRepository supervisorRepository,
        IInterestRepository interestRepository,
        IProposalRepository proposalRepository,
        ApplicationDbContext context)
    {
        _supervisorRepository = supervisorRepository;
        _interestRepository   = interestRepository;
        _proposalRepository   = proposalRepository;
        _context              = context;
    }

    public async Task<Result> ExpressInterestAsync(string supervisorId, int proposalId)
    {
        if (await _interestRepository.ExistsAsync(supervisorId, proposalId))
            return Result.Failure("You have already expressed interest in this proposal.");

        if (!await _supervisorRepository.HasCapacityAsync(supervisorId))
            return Result.Failure("You have reached your maximum project capacity.");

        var proposal = await _proposalRepository.GetByIdAsync(proposalId);
        if (proposal == null)
            return Result.Failure("Proposal not found.");

        var interest = new SupervisorInterest
        {
            SupervisorId = supervisorId,
            ProposalId   = proposalId,
            Status       = Core.Enums.InterestStatus.Pending,
            CreatedAt    = DateTime.UtcNow
        };

        await _interestRepository.AddAsync(interest);
        return Result.Success();
    }

    public async Task<Result> UpdateResearchAreasAsync(string supervisorId, List<int> researchAreaIds)
    {
        var supervisor = await _context.Supervisors
            .Include(s => s.PreferredResearchAreas)
            .FirstOrDefaultAsync(s => s.Id == supervisorId);

        if (supervisor == null)
            return Result.Failure("Supervisor not found.");

        var selectedAreas = await _context.ResearchAreas
            .Where(ra => researchAreaIds.Contains(ra.Id) && ra.IsActive)
            .ToListAsync();

        supervisor.PreferredResearchAreas.Clear();
        foreach (var area in selectedAreas)
            supervisor.PreferredResearchAreas.Add(area);

        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
