namespace BlindMatch.Web.ViewModels.ModuleLeader;

public class AuditLogFilterViewModel
{
    public string? ActionFilter { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<AuditLogRowViewModel> Events { get; set; } = new();
}

public class AuditLogRowViewModel
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Who { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
}