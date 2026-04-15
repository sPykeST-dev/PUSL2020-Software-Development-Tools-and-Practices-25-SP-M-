namespace BlindMatch.Core.Interfaces.Services;

public interface IAuditService
{
    Task LogAsync(
        string action,
        string entityName,
        string? entityId = null,
        string? userId = null,
        string? details = null);
}