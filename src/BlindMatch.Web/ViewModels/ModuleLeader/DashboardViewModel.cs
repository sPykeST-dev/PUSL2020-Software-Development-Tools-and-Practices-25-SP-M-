namespace BlindMatch.Web.ViewModels.ModuleLeader;

public class DashboardViewModel
{
    public int TotalProposals { get; set; }
    public Dictionary<string, int> ProposalsByStatus { get; set; } = new();
    public int PendingMatches { get; set; }
    public int ActiveMatches { get; set; }
    public List<AuditActivityRow> RecentActivity { get; set; } = new();

    public class AuditActivityRow
    {
        public DateTime When { get; set; }
        public string Who { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}