using BlindMatch.Core.Entities;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Match = BlindMatch.Core.Entities.Match;

namespace BlindMatch.Tests.IntegrationTests.Infrastructure;

/// <summary>
/// Factory for fresh InMemory <see cref="ApplicationDbContext"/> instances.
/// Each call to <see cref="CreateContext"/> with a unique name gives an isolated,
/// empty database. ResearchAreas 1–7 are always present via HasData seed.
/// </summary>
public class TestDatabaseFixture : IDisposable
{
    public ApplicationDbContext CreateContext(string? dbName = null)
    {
        var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var ctx = new ApplicationDbContext(opts);
        ctx.Database.EnsureCreated(); // applies HasData seeds (ResearchAreas 1–7)
        return ctx;
    }

    // ── Synchronous seed helpers ──────────────────────────────────────────────

    public static void SeedStudent(ApplicationDbContext ctx, Student student)
    {
        ctx.Students.Add(student);
        ctx.SaveChanges();
    }

    public static void SeedSupervisor(ApplicationDbContext ctx, Supervisor supervisor)
    {
        ctx.Supervisors.Add(supervisor);
        ctx.SaveChanges();
    }

    public static void SeedProposal(ApplicationDbContext ctx, Proposal proposal)
    {
        ctx.Proposals.Add(proposal);
        ctx.SaveChanges();
    }

    public static void SeedInterest(ApplicationDbContext ctx, SupervisorInterest interest)
    {
        ctx.SupervisorInterests.Add(interest);
        ctx.SaveChanges();
    }

    public static void SeedMatch(ApplicationDbContext ctx, Match match)
    {
        ctx.Matches.Add(match);
        ctx.SaveChanges();
    }

    public void Dispose() { }
}
