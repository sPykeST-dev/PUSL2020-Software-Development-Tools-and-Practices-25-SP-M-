using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;

namespace BlindMatch.Tests.TestHelpers.EntityBuilders;

public class ProposalBuilder
{
    private readonly Proposal _proposal = new()
    {
        Title          = "Test Proposal Title With Enough Characters",
        Abstract       = "This is a test abstract that meets the minimum character requirement for testing purposes.",
        TechnicalStack = "C#, .NET 9, SQL Server",
        Keywords       = "test, software, research",
        ResearchAreaId = 1,
        Status         = ProposalStatus.Submitted,
        SubmittedAt    = DateTime.UtcNow,
        UpdatedAt      = DateTime.UtcNow
    };

    public ProposalBuilder WithId(int id)
    {
        _proposal.Id = id;
        return this;
    }

    public ProposalBuilder WithStudentId(string studentId)
    {
        _proposal.StudentId = studentId;
        return this;
    }

    public ProposalBuilder WithStatus(ProposalStatus status)
    {
        _proposal.Status = status;
        return this;
    }

    public ProposalBuilder WithResearchAreaId(int id)
    {
        _proposal.ResearchAreaId = id;
        return this;
    }

    public ProposalBuilder AsMatched()   => WithStatus(ProposalStatus.Matched);
    public ProposalBuilder AsWithdrawn() => WithStatus(ProposalStatus.Withdrawn);

    public Proposal Build() => _proposal;
}

public class SupervisorInterestBuilder
{
    private readonly SupervisorInterest _interest = new()
    {
        SupervisorId = "sup-default",
        ProposalId   = 1,
        Status       = InterestStatus.Pending,
        CreatedAt    = DateTime.UtcNow
    };

    public SupervisorInterestBuilder WithId(int id)
    {
        _interest.Id = id;
        return this;
    }

    public SupervisorInterestBuilder WithSupervisorId(string id)
    {
        _interest.SupervisorId = id;
        return this;
    }

    public SupervisorInterestBuilder WithProposalId(int id)
    {
        _interest.ProposalId = id;
        return this;
    }

    public SupervisorInterestBuilder WithStatus(InterestStatus status)
    {
        _interest.Status = status;
        return this;
    }

    public SupervisorInterestBuilder WithProposal(Proposal p)
    {
        _interest.Proposal   = p;
        _interest.ProposalId = p.Id;
        return this;
    }

    public SupervisorInterestBuilder AlreadyConfirmed()
    {
        _interest.Status      = InterestStatus.Confirmed;
        _interest.ConfirmedAt = DateTime.UtcNow;
        return this;
    }

    public SupervisorInterest Build() => _interest;
}
