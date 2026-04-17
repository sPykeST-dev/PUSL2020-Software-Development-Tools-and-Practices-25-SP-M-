using BlindMatch.Core.Entities;

namespace BlindMatch.Tests.UnitTests.Core;

public class ProposalEntityTests
{
    // ── TC-PROP-01 ────────────────────────────────────────────────────────────

    [Fact]
    public void NewProposal_HasNullWithdrawnAt()
    {
        var proposal = new Proposal();

        proposal.WithdrawnAt.Should().BeNull();
    }

    // ── TC-PROP-02 ────────────────────────────────────────────────────────────

    [Fact]
    public void NewProposal_HasNonNullUpdatedAt()
    {
        var before   = DateTime.UtcNow.AddSeconds(-1);
        var proposal = new Proposal();

        proposal.UpdatedAt.Should().BeOnOrAfter(before);
    }

    // ── TC-PROP-03 ────────────────────────────────────────────────────────────

    [Fact]
    public void NewProposal_HasEmptyInterestsCollection()
    {
        var proposal = new Proposal();

        proposal.Interests.Should().NotBeNull();
        proposal.Interests.Should().BeEmpty();
    }
}
