using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;

namespace BlindMatch.Core.Interfaces.Repositories;

public interface IMatchRepository : IRepository<Match>
{
    Task<ICollection<Match>> GetByStatusAsync(MatchStatus status);
    Task<Match?> GetByProposalAndSupervisorAsync(int proposalId, string supervisorId);
    Task<int> CountByStatusAsync(MatchStatus status);
}