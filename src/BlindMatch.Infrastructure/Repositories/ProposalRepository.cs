using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Infrastructure.Repositories;

public class ProposalRepository : Repository<Proposal>, IProposalRepository
{
    public ProposalRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Proposal?> GetByStudentIdAsync(string studentId)
    {
        return await _context.Proposals
            .Include(p => p.ResearchArea)
            .FirstOrDefaultAsync(p => p.StudentId == studentId);
    }

    public async Task<Proposal?> GetByIdForStudentAsync(int proposalId, string studentId)
    {
        return await _context.Proposals
            .Include(p => p.ResearchArea)
            .FirstOrDefaultAsync(p => p.Id == proposalId && p.StudentId == studentId);
    }

    public async Task<bool> StudentHasProposalAsync(string studentId)
    {
        return await _context.Proposals.AnyAsync(p => p.StudentId == studentId);
    }

    public async Task<List<Proposal>> GetAllSubmittedAsync()
    {
        return await _context.Proposals
            .Include(p => p.ResearchArea)
            .Where(p => p.SubmittedAt.HasValue)
            .OrderByDescending(p => p.SubmittedAt)
            .ToListAsync();
    }

    public async Task<Proposal?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Proposals
            .Include(p => p.ResearchArea)
            .Include(p => p.Student)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Proposal>> GetAllWithDetailsAsync()
    {
        return await _context.Proposals
            .Include(p => p.ResearchArea)
            .Include(p => p.Student)
            .OrderByDescending(p => p.SubmittedAt)
            .ToListAsync();
    }
}
