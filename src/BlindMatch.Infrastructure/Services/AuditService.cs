using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.Interfaces.Services;

namespace BlindMatch.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task LogAsync(
        string action,
        string entityName,
        string? entityId = null,
        string? userId = null,
        string? details = null)
    {
        var entry = new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            UserId = userId,
            Details = details,
            OccurredAt = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(entry);
    }
}