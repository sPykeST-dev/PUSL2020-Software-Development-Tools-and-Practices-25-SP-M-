using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;

namespace BlindMatch.Core.Interfaces.Repositories;

public interface IMatchRepository : IRepository<Match>
{
    Task<ICollection<Match>> GetByStatusAsync(MatchStatus status);
    Task<Match?> GetByProposalAndSupervisorAsync(int proposalId, string supervisorId);
    Task<int> CountByStatusAsync(MatchStatus status);

    Task<Match?> GetByProposalIdWithDetailsAsync(int proposalId);

    Task<IEnumerable<Match>> GetBySupervisorIdWithDetailsAsync(string supervisorId);

    // Blocks duplicate confirms: true if a match already exists for this proposal.
    Task<bool> ExistsForProposalAsync(int proposalId);
}
