using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Infrastructure.Repositories;

public class InterestRepository : Repository<SupervisorInterest>, IInterestRepository
{
    public InterestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SupervisorInterest?> GetBySupervisorAndProposalAsync(string supervisorId, int proposalId)
    {
        return await _context.SupervisorInterests
            .Include(si => si.Supervisor)
            .Include(si => si.Proposal)
            .FirstOrDefaultAsync(si => si.SupervisorId == supervisorId && si.ProposalId == proposalId);
    }

    public async Task<ICollection<SupervisorInterest>> GetInterestsBySupervisorAsync(string supervisorId)
    {
        return await _context.SupervisorInterests
            .Include(si => si.Proposal)
                .ThenInclude(p => p.ResearchArea)
            .Where(si => si.SupervisorId == supervisorId)
            .OrderByDescending(si => si.CreatedAt)
            .ToListAsync();
    }

    public async Task<ICollection<SupervisorInterest>> GetPendingInterestsBySupervisorAsync(string supervisorId)
    {
        return await _context.SupervisorInterests
            .Include(si => si.Proposal)
                .ThenInclude(p => p.ResearchArea)
            .Where(si => si.SupervisorId == supervisorId && si.Status == InterestStatus.Pending)
            .OrderByDescending(si => si.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string supervisorId, int proposalId)
    {
        return await _context.SupervisorInterests
            .AnyAsync(si => si.SupervisorId == supervisorId && si.ProposalId == proposalId);
    }

    public async Task ConfirmInterestAsync(int interestId)
    {
        var interest = await _context.SupervisorInterests.FindAsync(interestId);
        if (interest != null && interest.Status == InterestStatus.Pending)
        {
            interest.Status = InterestStatus.Confirmed;
            interest.ConfirmedAt = DateTime.UtcNow;
            _context.SupervisorInterests.Update(interest);
            await _context.SaveChangesAsync();
        }
    }
}
