using BlindMatch.Core.Entities;

namespace BlindMatch.Core.Interfaces.Repositories;

public interface IIdentityRevealRepository : IRepository<IdentityReveal>
{
    Task<IdentityReveal?> GetByMatchIdAsync(int matchId);
}