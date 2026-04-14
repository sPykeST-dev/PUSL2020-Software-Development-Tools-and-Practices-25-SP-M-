using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;

namespace BlindMatch.Infrastructure.Repositories;

public class MatchRepository : Repository<Match>, IMatchRepository
{
    public MatchRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ICollection<Match>> GetByStatusAsync(MatchStatus status)
    {
        return await _context.Matches
            .Where(m => m.Status == status)
            .Include(m => m.Student)
            .Include(m => m.Supervisor)
            .Include(m => m.Proposal)
                .ThenInclude(p => p.ResearchArea)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Match?> GetByProposalAndSupervisorAsync(int proposalId, string supervisorId)
    {
        return await _context.Matches
            .Include(m => m.Student)
            .Include(m => m.Supervisor)
            .Include(m => m.Proposal)
            .FirstOrDefaultAsync(m => m.ProposalId == proposalId && m.SupervisorId == supervisorId);
    }

    public async Task<int> CountByStatusAsync(MatchStatus status)
    {
        return await _context.Matches.CountAsync(m => m.Status == status);
    }
}