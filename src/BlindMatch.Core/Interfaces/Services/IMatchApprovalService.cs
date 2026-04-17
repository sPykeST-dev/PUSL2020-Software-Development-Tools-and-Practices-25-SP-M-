using BlindMatch.Core.Enums;
using BlindMatch.Core.ValueObjects;

namespace BlindMatch.Core.Interfaces.Services;

public interface IMatchApprovalService
{
    Task<Result> ApproveMatchAsync(int matchId, string? userId = null, string? userFullName = null);
    Task<Result> RejectMatchAsync(int matchId, string reason, string? userId = null, string? userFullName = null);
    Task<Result> ReassignMatchAsync(int matchId, string newSupervisorId, string? userId = null, string? userFullName = null);
    Task<Result> ChangeProposalStatusAsync(int proposalId, ProposalStatus newStatus, string? userId = null, string? userFullName = null);
}
