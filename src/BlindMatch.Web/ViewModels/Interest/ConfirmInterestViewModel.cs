namespace BlindMatch.Web.ViewModels.Interest;

public class ConfirmInterestViewModel
{
    public int InterestId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SupervisorName { get; set; } = string.Empty;
    public string SupervisorEmail { get; set; } = string.Empty;
    public string SupervisorDepartment { get; set; } = string.Empty;
}
