using BlindMatch.Core.Entities;

namespace BlindMatch.Tests.UnitTests.Core;

public class ProposalEntityTests
{
    [Fact]
    public void NewProposal_HasNullWithdrawnAt()
    {
        var proposal = new Proposal();

        proposal.WithdrawnAt.Should().BeNull();
    }

    [Fact]
    public void NewProposal_HasNonNullUpdatedAt()
    {
        var before   = DateTime.UtcNow.AddSeconds(-1);
        var proposal = new Proposal();

        proposal.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void NewProposal_HasEmptyInterestsCollection()
    {
        var proposal = new Proposal();

        proposal.Interests.Should().NotBeNull();
        proposal.Interests.Should().BeEmpty();
    }
}
