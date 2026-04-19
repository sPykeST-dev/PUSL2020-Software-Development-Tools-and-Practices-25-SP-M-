using System.Linq.Expressions;
using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Services;
using BlindMatch.Tests.TestHelpers.EntityBuilders;

namespace BlindMatch.Tests.UnitTests.Services;

public class ProposalServiceTests
{
    private readonly Mock<IProposalRepository>      _repo     = new();
    private readonly Mock<IRepository<ResearchArea>> _areaRepo = new();

    private ProposalService CreateService() => new(_repo.Object, _areaRepo.Object);

    private void SetupAreaExists(bool exists = true) =>
        _areaRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ResearchArea, bool>>>()))
                 .ReturnsAsync(exists);

    private void SetupStudentHasProposal(bool hasProposal) =>
        _repo.Setup(r => r.StudentHasProposalAsync(It.IsAny<string>()))
             .ReturnsAsync(hasProposal);

    private void SetupGetByIdForStudent(Proposal? proposal) =>
        _repo.Setup(r => r.GetByIdForStudentAsync(It.IsAny<int>(), It.IsAny<string>()))
             .ReturnsAsync(proposal);

    [Fact]
    public async Task Create_EmptyStudentId_ReturnsFailure()
    {
        var result = await CreateService().CreateAsync("", new ProposalBuilder().Build());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Student identity is required.");
    }

    [Fact]
    public async Task Create_WhitespaceStudentId_ReturnsFailure()
    {
        var result = await CreateService().CreateAsync("   ", new ProposalBuilder().Build());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Student identity is required.");
    }

    [Fact]
    public async Task Create_StudentAlreadyHasProposal_ReturnsFailure()
    {
        SetupStudentHasProposal(true);

        var result = await CreateService().CreateAsync("stu-1", new ProposalBuilder().Build());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only one proposal is allowed per student.");
    }

    [Fact]
    public async Task Create_InvalidResearchArea_ReturnsFailure()
    {
        SetupStudentHasProposal(false);
        SetupAreaExists(false);

        var result = await CreateService().CreateAsync("stu-1",
            new ProposalBuilder().WithResearchAreaId(99).Build());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Please select a valid research area.");
    }

    [Fact]
    public async Task Create_ValidInput_SetsRequiredFields()
    {
        SetupStudentHasProposal(false);
        SetupAreaExists(true);
        _repo.Setup(r => r.AddAsync(It.IsAny<Proposal>())).Returns(Task.CompletedTask);

        var before = DateTime.UtcNow;
        var result = await CreateService().CreateAsync("stu-1", new ProposalBuilder().Build());

        result.IsSuccess.Should().BeTrue();
        _repo.Verify(r => r.AddAsync(It.Is<Proposal>(p =>
            p.StudentId == "stu-1"           &&
            p.Status    == ProposalStatus.Submitted &&
            p.SubmittedAt >= before           &&
            p.WithdrawnAt == null
        )), Times.Once);
    }

    [Fact]
    public async Task Update_ProposalNotFound_ReturnsFailure()
    {
        SetupGetByIdForStudent(null);

        var result = await CreateService().UpdateAsync("stu-1", new ProposalBuilder().Build());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Proposal not found.");
    }

    [Fact]
    public async Task Update_MatchedProposal_ReturnsFailure()
    {
        SetupGetByIdForStudent(new ProposalBuilder().AsMatched().Build());

        var result = await CreateService().UpdateAsync("stu-1", new ProposalBuilder().Build());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("You cannot edit a proposal after it has been matched.");
    }

    [Fact]
    public async Task Update_WithdrawnProposal_ReturnsFailure()
    {
        SetupGetByIdForStudent(new ProposalBuilder().AsWithdrawn().Build());

        var result = await CreateService().UpdateAsync("stu-1", new ProposalBuilder().Build());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("You cannot edit a withdrawn proposal.");
    }

    [Fact]
    public async Task Update_InvalidResearchArea_ReturnsFailure()
    {
        SetupGetByIdForStudent(new ProposalBuilder().Build());
        SetupAreaExists(false);

        var result = await CreateService().UpdateAsync("stu-1", new ProposalBuilder().WithResearchAreaId(99).Build());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Please select a valid research area.");
    }

    [Fact]
    public async Task Update_ValidInput_UpdatesAllowedFields()
    {
        SetupGetByIdForStudent(new ProposalBuilder().Build());
        SetupAreaExists(true);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Proposal>())).Returns(Task.CompletedTask);

        var updated = new ProposalBuilder()
            .WithResearchAreaId(2)
            .Build();
        updated.Title    = "Updated Title Text";
        updated.Abstract = "Updated abstract with enough text to pass validation requirements.";

        var before = DateTime.UtcNow;
        var result = await CreateService().UpdateAsync("stu-1", updated);

        result.IsSuccess.Should().BeTrue();
        _repo.Verify(r => r.UpdateAsync(It.Is<Proposal>(p =>
            p.Title           == "Updated Title Text" &&
            p.ResearchAreaId  == 2                    &&
            p.UpdatedAt       >= before
        )), Times.Once);
    }

    [Fact]
    public async Task Withdraw_ProposalNotFound_ReturnsFailure()
    {
        SetupGetByIdForStudent(null);

        var result = await CreateService().WithdrawAsync("stu-1", proposalId: 1);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Proposal not found.");
    }

    [Fact]
    public async Task Withdraw_MatchedProposal_ReturnsFailure()
    {
        SetupGetByIdForStudent(new ProposalBuilder().AsMatched().Build());

        var result = await CreateService().WithdrawAsync("stu-1", proposalId: 1);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("You cannot withdraw a proposal after it has been matched.");
    }

    [Fact]
    public async Task Withdraw_AlreadyWithdrawn_ReturnsFailure()
    {
        SetupGetByIdForStudent(new ProposalBuilder().AsWithdrawn().Build());

        var result = await CreateService().WithdrawAsync("stu-1", proposalId: 1);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This proposal has already been withdrawn.");
    }

    [Fact]
    public async Task Withdraw_ValidInput_SetsWithdrawnStatus()
    {
        SetupGetByIdForStudent(new ProposalBuilder().Build());
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Proposal>())).Returns(Task.CompletedTask);

        var before = DateTime.UtcNow;
        var result = await CreateService().WithdrawAsync("stu-1", proposalId: 1);

        result.IsSuccess.Should().BeTrue();
        _repo.Verify(r => r.UpdateAsync(It.Is<Proposal>(p =>
            p.Status      == ProposalStatus.Withdrawn &&
            p.WithdrawnAt >= before                   &&
            p.UpdatedAt   >= before
        )), Times.Once);
    }
}
