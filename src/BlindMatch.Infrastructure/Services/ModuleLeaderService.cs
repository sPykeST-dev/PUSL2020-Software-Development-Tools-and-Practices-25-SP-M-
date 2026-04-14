using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.ValueObjects;

namespace BlindMatch.Infrastructure.Services;

public class ModuleLeaderService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IProposalRepository _proposalRepository;
    private readonly ISupervisorRepository _supervisorRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IInterestRepository _interestRepository;

    public ModuleLeaderService(
        IMatchRepository matchRepository,
        IProposalRepository proposalRepository,
        ISupervisorRepository supervisorRepository,
        IAuditRepository auditRepository,
        IInterestRepository interestRepository)
    {
        _matchRepository = matchRepository;
        _proposalRepository = proposalRepository;
        _supervisorRepository = supervisorRepository;
        _auditRepository = auditRepository;
        _interestRepository = interestRepository;
    }

    public async Task<Result> ApproveMatchAsync(int matchId, string? userId = null, string? userFullName = null)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            return Result.Failure("Match not found.");

        if (match.Status != MatchStatus.Pending)
            return Result.Failure("Only pending matches can be approved.");

        match.Status = MatchStatus.Active;
        match.ApprovedAt = DateTime.UtcNow;
        await _matchRepository.UpdateAsync(match);

        // Update proposal status
        var proposal = await _proposalRepository.GetByIdAsync(match.ProposalId);
        if (proposal != null)
        {
            proposal.Status = ProposalStatus.Matched;
            await _proposalRepository.UpdateAsync(proposal);
        }

        // Increment supervisor project count
        var supervisor = await _supervisorRepository.GetByIdAsync(match.SupervisorId);
        if (supervisor != null)
        {
            supervisor.CurrentProjects++;
            await _supervisorRepository.UpdateAsync(supervisor);
        }

        // Audit log
        await _auditRepository.AddAsync(new AuditEvent
        {
            Action = "MatchApproved",
            UserId = userId,
            UserFullName = userFullName,
            Timestamp = DateTime.UtcNow,
            Details = $"Approved match {matchId} for proposal {match.ProposalId} with supervisor {match.SupervisorId}"
        });

        return Result.Success();
    }

    public async Task<Result> RejectMatchAsync(int matchId, string reason, string? userId = null, string? userFullName = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure("Rejection reason is required.");

        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            return Result.Failure("Match not found.");

        if (match.Status != MatchStatus.Pending)
            return Result.Failure("Only pending matches can be rejected.");

        match.Status = MatchStatus.Rejected;
        match.RejectedAt = DateTime.UtcNow;
        match.RejectionReason = reason.Trim();
        await _matchRepository.UpdateAsync(match);

        // Revert proposal to Submitted
        var proposal = await _proposalRepository.GetByIdAsync(match.ProposalId);
        if (proposal != null)
        {
            proposal.Status = ProposalStatus.Submitted;
            await _proposalRepository.UpdateAsync(proposal);

            // Clear interests for this proposal to allow re-browsing
            var interests = await _interestRepository.FindAsync(i => i.ProposalId == match.ProposalId);
            foreach (var interest in interests)
            {
                if (interest.Status == Core.Enums.InterestStatus.Confirmed)
                {
                    interest.Status = Core.Enums.InterestStatus.Pending;
                    await _interestRepository.UpdateAsync(interest);
                }
            }
        }

        // Decrement supervisor project count
        var supervisor = await _supervisorRepository.GetByIdAsync(match.SupervisorId);
        if (supervisor != null && supervisor.CurrentProjects > 0)
        {
            supervisor.CurrentProjects--;
            await _supervisorRepository.UpdateAsync(supervisor);
        }

        // Audit log
        await _auditRepository.AddAsync(new AuditEvent
        {
            Action = "MatchRejected",
            UserId = userId,
            UserFullName = userFullName,
            Timestamp = DateTime.UtcNow,
            Details = $"Rejected match {matchId} for proposal {match.ProposalId}: {reason}"
        });

        return Result.Success();
    }

    public async Task<Result> ReassignMatchAsync(int matchId, string newSupervisorId, string? userId = null, string? userFullName = null)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            return Result.Failure("Match not found.");

        if (match.SupervisorId == newSupervisorId)
            return Result.Failure("Selected supervisor is already assigned to this match.");

        var oldSupervisorId = match.SupervisorId;

        // Decrement old supervisor
        var oldSupervisor = await _supervisorRepository.GetByIdAsync(oldSupervisorId);
        if (oldSupervisor != null && oldSupervisor.CurrentProjects > 0)
        {
            oldSupervisor.CurrentProjects--;
            await _supervisorRepository.UpdateAsync(oldSupervisor);
        }

        // Assign new supervisor
        match.SupervisorId = newSupervisorId;
        await _matchRepository.UpdateAsync(match);

        // Increment new supervisor
        var newSupervisor = await _supervisorRepository.GetByIdAsync(newSupervisorId);
        if (newSupervisor != null)
        {
            newSupervisor.CurrentProjects++;
            await _supervisorRepository.UpdateAsync(newSupervisor);
        }

        // Audit log
        await _auditRepository.AddAsync(new AuditEvent
        {
            Action = "MatchReassigned",
            UserId = userId,
            UserFullName = userFullName,
            Timestamp = DateTime.UtcNow,
            Details = $"Reassigned match {matchId} from {oldSupervisorId} to {newSupervisorId}"
        });

        return Result.Success();
    }

    public async Task<Result> ChangeProposalStatusAsync(int proposalId, ProposalStatus newStatus, string? userId = null, string? userFullName = null)
    {
        var proposal = await _proposalRepository.GetByIdAsync(proposalId);
        if (proposal == null)
            return Result.Failure("Proposal not found.");

        var oldStatus = proposal.Status;
        proposal.Status = newStatus;
        await _proposalRepository.UpdateAsync(proposal);

        // Audit log
        await _auditRepository.AddAsync(new AuditEvent
        {
            Action = "ProposalStatusChanged",
            UserId = userId,
            UserFullName = userFullName,
            Timestamp = DateTime.UtcNow,
            Details = $"Changed proposal {proposalId} status from {oldStatus} to {newStatus}"
        });

        return Result.Success();
    }
}