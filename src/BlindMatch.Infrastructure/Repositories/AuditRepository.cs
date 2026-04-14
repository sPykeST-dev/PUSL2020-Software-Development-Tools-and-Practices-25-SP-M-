using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;

namespace BlindMatch.Infrastructure.Repositories;

public class AuditRepository : Repository<AuditEvent>, IAuditRepository
{
    public AuditRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ICollection<AuditEvent>> GetRecentAsync(int count)
    {
        return await _context.AuditEvents
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<ICollection<AuditEvent>> GetByUserAsync(string userId)
    {
        return await _context.AuditEvents
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<ICollection<AuditEvent>> FilterAsync(string? action = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.AuditEvents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action.Contains(action));

        if (startDate.HasValue)
            query = query.Where(a => a.Timestamp >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.Timestamp <= endDate.Value);

        return await query.OrderByDescending(a => a.Timestamp).ToListAsync();
    }
}