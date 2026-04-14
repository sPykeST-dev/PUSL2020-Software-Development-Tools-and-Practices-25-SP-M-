using BlindMatch.Core.Entities;
using BlindMatch.Core.ValueObjects;

namespace BlindMatch.Core.Interfaces.Services;

public interface IProposalService
{
	Task<Proposal?> GetStudentProposalAsync(string studentId);
	Task<ICollection<ResearchArea>> GetActiveResearchAreasAsync();
	Task<Result> CreateAsync(string studentId, Proposal proposal);
	Task<Result> UpdateAsync(string studentId, Proposal proposal);
	Task<Result> WithdrawAsync(string studentId, int proposalId);
}
