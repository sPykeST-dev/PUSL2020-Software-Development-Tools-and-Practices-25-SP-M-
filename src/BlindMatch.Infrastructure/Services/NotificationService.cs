using BlindMatch.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BlindMatch.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyMatchCreatedAsync(string studentId, string supervisorId, int proposalId)
    {
        _logger.LogInformation(
            "[NOTIFICATION] Match created for Proposal #{ProposalId}. " +
            "Student: {StudentId} | Supervisor: {SupervisorId}. " +
            "Both parties can now view each other's contact details.",
            proposalId, studentId, supervisorId);

        return Task.CompletedTask;
    }
}