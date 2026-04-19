using BlindMatch.Core.Entities;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Match = BlindMatch.Core.Entities.Match;

namespace BlindMatch.Tests.IntegrationTests.Infrastructure;

public class TestDatabaseFixture : IDisposable
{
    public ApplicationDbContext CreateContext(string? dbName = null)
    {
        var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var ctx = new ApplicationDbContext(opts);
        ctx.Database.EnsureCreated();
        return ctx;
    }

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
