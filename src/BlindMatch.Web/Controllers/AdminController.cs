using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlindMatch.Web.Controllers;

[Authorize(Roles = "SystemAdministrator")]
public class AdminController : Controller
{
    public IActionResult Index()    => RedirectToPage("/Admin/AuditLog");
    public IActionResult AuditLog() => RedirectToPage("/Admin/AuditLog");
}
