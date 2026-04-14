using BlindMatch.Core.Entities;

namespace BlindMatch.Core.Interfaces.Repositories;

public interface IAuditRepository : IRepository<AuditEvent>
{
    Task<ICollection<AuditEvent>> GetRecentAsync(int count);
    Task<ICollection<AuditEvent>> GetByUserAsync(string userId);
    Task<ICollection<AuditEvent>> FilterAsync(string? action = null, DateTime? startDate = null, DateTime? endDate = null);
}