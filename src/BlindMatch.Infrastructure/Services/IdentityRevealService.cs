using BlindMatch.Core.Interfaces.Services;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Infrastructure.Services;

public class IdentityRevealService : IIdentityRevealService
{
    private readonly ApplicationDbContext _context;

    public IdentityRevealService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasRevealOccurredAsync(int proposalId)
    {
        return await _context.Matches
            .AnyAsync(m => m.ProposalId == proposalId && m.IdentityReveal != null);
    }

    public async Task<RevealedSupervisorDetails?> GetSupervisorDetailsForStudentAsync(
        int proposalId, string studentId)
    {
        var match = await _context.Matches
            .Include(m => m.Supervisor)
            .Include(m => m.IdentityReveal)
            .FirstOrDefaultAsync(m =>
                m.ProposalId == proposalId &&
                m.StudentId == studentId &&
                m.IdentityReveal != null);

        if (match == null) return null;

        return new RevealedSupervisorDetails(
            match.Supervisor.FullName,
            match.Supervisor.Email!,
            match.Supervisor.Department);
    }

    public async Task<RevealedStudentDetails?> GetStudentDetailsForSupervisorAsync(
        int matchId, string supervisorId)
    {
        var match = await _context.Matches
            .Include(m => m.Student)
            .Include(m => m.IdentityReveal)
            .FirstOrDefaultAsync(m =>
                m.Id == matchId &&
                m.SupervisorId == supervisorId &&
                m.IdentityReveal != null);

        if (match == null) return null;

        return new RevealedStudentDetails(
            match.Student.FullName,
            match.Student.Email!,
            match.Student.Programme,
            match.Student.YearOfStudy);
    }
}