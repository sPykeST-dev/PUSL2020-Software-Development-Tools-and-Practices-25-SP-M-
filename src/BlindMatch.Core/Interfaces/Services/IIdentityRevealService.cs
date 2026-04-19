namespace BlindMatch.Core.Interfaces.Services;

public interface IIdentityRevealService
{
    // Gates whether identity information may be shown to either party.
    Task<bool> HasRevealOccurredAsync(int proposalId);

    Task<RevealedSupervisorDetails?> GetSupervisorDetailsForStudentAsync(int proposalId, string studentId);

    Task<RevealedStudentDetails?> GetStudentDetailsForSupervisorAsync(int matchId, string supervisorId);
}

public record RevealedSupervisorDetails(string FullName, string Email, string Department);

public record RevealedStudentDetails(string FullName, string Email, string? Programme, int? YearOfStudy);
