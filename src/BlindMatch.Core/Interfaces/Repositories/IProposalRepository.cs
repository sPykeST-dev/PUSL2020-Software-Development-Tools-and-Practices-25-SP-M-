using BlindMatch.Core.Entities;

namespace BlindMatch.Core.Interfaces.Repositories;

public interface IProposalRepository : IRepository<Proposal>
{
    Task<Proposal?> GetByStudentIdAsync(string studentId);
    Task<Proposal?> GetByIdForStudentAsync(int proposalId, string studentId);
    Task<bool> StudentHasProposalAsync(string studentId);
}
