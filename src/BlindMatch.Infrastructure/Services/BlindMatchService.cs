using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.Interfaces.Services;
using BlindMatch.Core.ValueObjects;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlindMatch.Infrastructure.Services;

public class BlindMatchService : IBlindMatchService
{
    private readonly ApplicationDbContext _context;
    private readonly ISupervisorRepository _supervisorRepository;
    private readonly INotificationService _notificationService;
    private readonly IAuditService _auditService;
    private readonly ILogger<BlindMatchService> _logger;

    public BlindMatchService(
        ApplicationDbContext context,
        ISupervisorRepository supervisorRepository,
        INotificationService notificationService,
        IAuditService auditService,
        ILogger<BlindMatchService> logger)
    {
        _context = context;
        _supervisorRepository = supervisorRepository;
        _notificationService = notificationService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result> ConfirmInterestAsync(int interestId, string supervisorId)
    {
        var interest = await _context.SupervisorInterests
            .Include(i => i.Proposal)
            .FirstOrDefaultAsync(i => i.Id == interestId);

        if (interest == null)
            return Result.Failure("Interest record not found.");

        if (interest.SupervisorId != supervisorId)
            return Result.Failure("You are not authorised to confirm this interest.");

        if (interest.Status == InterestStatus.Confirmed)
            return Result.Failure("This interest has already been confirmed.");

        if (interest.Proposal.Status == ProposalStatus.Matched)
            return Result.Failure("This proposal has already been matched with another supervisor.");

        var hasCapacity = await _supervisorRepository.HasCapacityAsync(supervisorId);
        if (!hasCapacity)
            return Result.Failure("You have reached your maximum project capacity and cannot take on more projects.");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            interest.Status = InterestStatus.Confirmed;
            interest.ConfirmedAt = DateTime.UtcNow;

            var match = new Match
            {
                ProposalId = interest.ProposalId,
                StudentId = interest.Proposal.StudentId,
                SupervisorId = supervisorId,
                Status = MatchStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _context.Matches.Add(match);

            // Use the navigation property so EF resolves the FK automatically
            // without needing match.Id before SaveChanges.
            var reveal = new IdentityReveal
            {
                Match = match,
                RevealedAt = DateTime.UtcNow,
                TriggeredBySupervisorId = supervisorId
            };
            _context.IdentityReveals.Add(reveal);

            interest.Proposal.Status = ProposalStatus.Matched;

            await _context.SaveChangesAsync();

            // IncrementProjectCountAsync may call SaveChanges internally;
            // that is fine inside a transaction.
            await _supervisorRepository.IncrementProjectCountAsync(supervisorId);

            await transaction.CommitAsync();

            _logger.LogInformation(
                "Identity reveal complete. Match #{MatchId} created for Proposal #{ProposalId}.",
                match.Id, interest.ProposalId);

            // Post-commit: notifications and audit run OUTSIDE the transaction
            // intentionally — a failed notification must not undo a successful match.
            await _notificationService.NotifyMatchCreatedAsync(
                interest.Proposal.StudentId, supervisorId, interest.ProposalId);

            await _auditService.LogAsync(
                action: "ConfirmInterest",
                entityName: "Match",
                entityId: match.Id.ToString(),
                userId: supervisorId,
                details: $"Interest #{interestId} confirmed. Match #{match.Id} created. Proposal #{interest.ProposalId} marked Matched.");

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "ConfirmInterest transaction failed for InterestId={InterestId}", interestId);
            return Result.Failure("An unexpected error occurred. No changes were saved. Please try again.");
        }
    }
}
