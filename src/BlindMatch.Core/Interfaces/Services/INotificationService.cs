namespace BlindMatch.Core.Interfaces.Services;

public interface INotificationService
{
    /// Notifies both parties that a match has been created and identities revealed.
    /// Currently logs to the application log. Can be upgraded to real email later.
    Task NotifyMatchCreatedAsync(string studentId, string supervisorId, int proposalId);
}