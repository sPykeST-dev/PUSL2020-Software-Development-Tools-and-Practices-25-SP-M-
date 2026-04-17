using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Infrastructure.Repositories;

public class IdentityRevealRepository : Repository<IdentityReveal>, IIdentityRevealRepository
{
    public IdentityRevealRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IdentityReveal?> GetByMatchIdAsync(int matchId)
    {
        return await _context.IdentityReveals
            .Include(ir => ir.Match)
            .FirstOrDefaultAsync(ir => ir.MatchId == matchId);
    }
}