using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Infrastructure.Data;
using BlindMatch.Infrastructure.Repositories;
using BlindMatch.Tests.IntegrationTests.Infrastructure;
using BlindMatch.Tests.TestHelpers.EntityBuilders;

namespace BlindMatch.Tests.IntegrationTests.Infrastructure;

public class ProposalRepositoryTests
{
    private static ApplicationDbContext CreateContext() =>
        new TestDatabaseFixture().CreateContext();

    [Fact]
    public async Task GetAllSubmitted_ReturnsOnlyProposalsWithSubmittedAt()
    {
        using var ctx  = CreateContext();
        var student1   = new StudentBuilder().WithId("stu-sub-1").Build();
        var student2   = new StudentBuilder().WithId("stu-sub-2").Build();
        ctx.Students.AddRange(student1, student2);

        ctx.Proposals.Add(new ProposalBuilder().WithId(10).WithStudentId("stu-sub-1")
            .WithStatus(ProposalStatus.Submitted).Build());
        ctx.Proposals.Add(new ProposalBuilder().WithId(11).WithStudentId("stu-sub-2")
            .WithStatus(ProposalStatus.Draft).Build());

        var draft = ctx.Proposals.Find(11)!;
        draft.SubmittedAt = null;
        await ctx.SaveChangesAsync();

        var repo   = new ProposalRepository(ctx);
        var result = await repo.GetAllSubmittedAsync();

        result.Should().HaveCount(1);
        result.Single().StudentId.Should().Be("stu-sub-1");
    }

    [Fact]
    public async Task GetAllSubmitted_OrdersBySubmittedAtDescending()
    {
        using var ctx = CreateContext();
        ctx.Students.AddRange(
            new StudentBuilder().WithId("stu-ord-1").Build(),
            new StudentBuilder().WithId("stu-ord-2").Build());

        var earlier = DateTime.UtcNow.AddHours(-2);
        var later   = DateTime.UtcNow;

        ctx.Proposals.Add(new ProposalBuilder().WithId(20).WithStudentId("stu-ord-1").Build());
        ctx.Proposals.Add(new ProposalBuilder().WithId(21).WithStudentId("stu-ord-2").Build());
        await ctx.SaveChangesAsync();

        ctx.Proposals.Find(20)!.SubmittedAt = earlier;
        ctx.Proposals.Find(21)!.SubmittedAt = later;
        await ctx.SaveChangesAsync();

        var repo   = new ProposalRepository(ctx);
        var result = await repo.GetAllSubmittedAsync();

        result.Should().HaveCount(2);
        result[0].StudentId.Should().Be("stu-ord-2");
        result[1].StudentId.Should().Be("stu-ord-1");
    }

    [Fact]
    public async Task StudentHasProposal_WhenExists_ReturnsTrue()
    {
        using var ctx = CreateContext();
        ctx.Students.Add(new StudentBuilder().WithId("stu-has-1").Build());
        ctx.Proposals.Add(new ProposalBuilder().WithId(30).WithStudentId("stu-has-1").Build());
        await ctx.SaveChangesAsync();

        var repo   = new ProposalRepository(ctx);
        var result = await repo.StudentHasProposalAsync("stu-has-1");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task StudentHasProposal_WhenNotExists_ReturnsFalse()
    {
        using var ctx = CreateContext();
        var repo      = new ProposalRepository(ctx);

        var result = await repo.StudentHasProposalAsync("stu-nobody");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdForStudent_CorrectOwner_ReturnsProposal()
    {
        using var ctx = CreateContext();
        ctx.Students.Add(new StudentBuilder().WithId("stu-own-1").Build());
        ctx.Proposals.Add(new ProposalBuilder().WithId(40).WithStudentId("stu-own-1").Build());
        await ctx.SaveChangesAsync();

        var repo   = new ProposalRepository(ctx);
        var result = await repo.GetByIdForStudentAsync(40, "stu-own-1");

        result.Should().NotBeNull();
        result!.Id.Should().Be(40);
    }

    [Fact]
    public async Task GetByIdForStudent_WrongStudentId_ReturnsNull()
    {
        using var ctx = CreateContext();
        ctx.Students.Add(new StudentBuilder().WithId("stu-own-2").Build());
        ctx.Proposals.Add(new ProposalBuilder().WithId(41).WithStudentId("stu-own-2").Build());
        await ctx.SaveChangesAsync();

        var repo   = new ProposalRepository(ctx);
        var result = await repo.GetByIdForStudentAsync(41, "stu-other");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByStudentId_IncludesResearchArea()
    {
        using var ctx = CreateContext();
        ctx.Students.Add(new StudentBuilder().WithId("stu-ra-1").Build());
        ctx.Proposals.Add(new ProposalBuilder().WithId(50).WithStudentId("stu-ra-1")
            .WithResearchAreaId(1).Build());
        await ctx.SaveChangesAsync();

        var repo   = new ProposalRepository(ctx);
        var result = await repo.GetByStudentIdAsync("stu-ra-1");

        result.Should().NotBeNull();
        result!.ResearchArea.Should().NotBeNull();
        result.ResearchArea!.Name.Should().Be("Artificial Intelligence");
    }
}
