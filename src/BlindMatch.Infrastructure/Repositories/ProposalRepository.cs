// Stub implementation - will be completed by Member 3
using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;

namespace BlindMatch.Infrastructure.Repositories;

public class ProposalRepository : Repository<Proposal>, IProposalRepository
{
    public ProposalRepository(ApplicationDbContext context) : base(context)
    {
    }

    // Methods will be implemented by Member 3
}
