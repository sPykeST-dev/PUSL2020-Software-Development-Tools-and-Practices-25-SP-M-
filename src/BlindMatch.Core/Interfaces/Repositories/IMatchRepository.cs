using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;

namespace BlindMatch.Core.Interfaces.Repositories;

public interface IMatchRepository : IRepository<Match>
{
    // used by the Module Leader dashboard
    Task<ICollection<Match>> GetByStatusAsync(MatchStatus status);
    Task<Match?> GetByProposalAndSupervisorAsync(int proposalId, string supervisorId);
    Task<int> CountByStatusAsync(MatchStatus status);

    // used by the blind match engine
    // Loads a match with its proposal, student, supervisor, and reveal.
    Task<Match?> GetByProposalIdWithDetailsAsync(int proposalId);

    // All matches for a supervisor, with full details.
    Task<IEnumerable<Match>> GetBySupervisorIdWithDetailsAsync(string supervisorId);

    // True if a match already exists for this proposal (blocks duplicate confirms).
    Task<bool> ExistsForProposalAsync(int proposalId);
}