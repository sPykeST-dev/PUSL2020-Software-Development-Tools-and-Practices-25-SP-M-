namespace BlindMatch.Core.Interfaces.Services;

public interface IIdentityRevealService
{
    // Returns true if a Match+IdentityReveal record exists for this proposal.
    // Used by other members to gate whether identity information is shown.
    Task<bool> HasRevealOccurredAsync(int proposalId);

    // Returns supervisor details for the student to see after reveal.
    Task<RevealedSupervisorDetails?> GetSupervisorDetailsForStudentAsync(int proposalId, string studentId);

    // Returns student details for the supervisor to see after reveal.
    Task<RevealedStudentDetails?> GetStudentDetailsForSupervisorAsync(int matchId, string supervisorId);
}

public record RevealedSupervisorDetails(string FullName, string Email, string Department);

public record RevealedStudentDetails(string FullName, string Email, string? Programme, int? YearOfStudy);