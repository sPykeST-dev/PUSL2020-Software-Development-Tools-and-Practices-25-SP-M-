using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Web.ViewModels.ModuleLeader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text;

namespace BlindMatch.Web.Pages.Admin;

[Authorize(Roles = "SystemAdministrator")]
public class AuditLogModel : PageModel
{
    private readonly IAuditRepository _auditRepository;

    public AuditLogModel(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string? ActionFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    public AuditLogFilterViewModel ViewModel { get; set; } = new();

    public async Task OnGetAsync()
    {
        var events = await _auditRepository.FilterAsync(ActionFilter, StartDate, EndDate);

        ViewModel.ActionFilter = ActionFilter;
        ViewModel.StartDate = StartDate;
        ViewModel.EndDate = EndDate;
        ViewModel.Events = events
            .Select(a => new AuditLogRowViewModel
            {
                Id = a.Id,
                Timestamp = a.Timestamp,
                Who = a.UserFullName ?? a.UserId ?? "System",
                Action = a.Action,
                Details = a.Details ?? string.Empty
            })
            .ToList();
    }

    public async Task<IActionResult> OnGetExportCsvAsync()
    {
        var events = await _auditRepository.FilterAsync(ActionFilter, StartDate, EndDate);

        var csv = new StringBuilder();
        csv.AppendLine("Timestamp,User,Action,Details");

        foreach (var evt in events)
        {
            var timestamp = evt.Timestamp.ToLocalTime().ToString("g", CultureInfo.InvariantCulture);
            var who = evt.UserFullName ?? evt.UserId ?? "System";
            var action = evt.Action.Replace("\"", "\"\"");
            var details = (evt.Details ?? "").Replace("\"", "\"\"");

            csv.AppendLine($"\"{timestamp}\",\"{who}\",\"{action}\",\"{details}\"");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }
}