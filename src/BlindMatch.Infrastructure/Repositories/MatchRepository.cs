using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Infrastructure.Repositories;

public class MatchRepository : Repository<Match>, IMatchRepository
{
    private readonly ApplicationDbContext _context;

    public MatchRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Match?> GetByProposalIdWithDetailsAsync(int proposalId)
    {
        return await _context.Matches
            .Include(m => m.Proposal)
            .Include(m => m.Student)
            .Include(m => m.Supervisor)
            .Include(m => m.IdentityReveal)
            .FirstOrDefaultAsync(m => m.ProposalId == proposalId);
    }

    public async Task<IEnumerable<Match>> GetBySupervisorIdWithDetailsAsync(string supervisorId)
    {
        return await _context.Matches
            .Include(m => m.Proposal)
            .Include(m => m.Student)
            .Include(m => m.Supervisor)
            .Include(m => m.IdentityReveal)
            .Where(m => m.SupervisorId == supervisorId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsForProposalAsync(int proposalId)
    {
        return await _context.Matches.AnyAsync(m => m.ProposalId == proposalId);
    }

    public async Task<ICollection<Match>> GetByStatusAsync(MatchStatus status)
    {
        return await _context.Matches
            .Include(m => m.Proposal)
            .Include(m => m.Student)
            .Include(m => m.Supervisor)
            .Include(m => m.IdentityReveal)
            .Where(m => m.Status == status)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Match?> GetByProposalAndSupervisorAsync(int proposalId, string supervisorId)
    {
        return await _context.Matches
            .FirstOrDefaultAsync(m => m.ProposalId == proposalId && m.SupervisorId == supervisorId);
    }

    public async Task<int> CountByStatusAsync(MatchStatus status)
    {
        return await _context.Matches.CountAsync(m => m.Status == status);
    }
}