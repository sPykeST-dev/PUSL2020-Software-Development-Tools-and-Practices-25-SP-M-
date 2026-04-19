namespace BlindMatch.Core.Interfaces.Services;

public interface INotificationService
{
    // Currently logs to the application log; can be upgraded to real email later.
    Task NotifyMatchCreatedAsync(string studentId, string supervisorId, int proposalId);
}
