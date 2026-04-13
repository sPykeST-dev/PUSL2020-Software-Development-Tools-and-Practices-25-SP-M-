using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;

namespace BlindMatch.Core.Interfaces.Repositories;

public interface IInterestRepository : IRepository<SupervisorInterest>
{
    Task<SupervisorInterest?> GetBySupervisorAndProposalAsync(string supervisorId, int proposalId);
    Task<ICollection<SupervisorInterest>> GetInterestsBySupervisorAsync(string supervisorId);
    Task<ICollection<SupervisorInterest>> GetPendingInterestsBySupervisorAsync(string supervisorId);
    Task<bool> ExistsAsync(string supervisorId, int proposalId);
    Task ConfirmInterestAsync(int interestId);
}
