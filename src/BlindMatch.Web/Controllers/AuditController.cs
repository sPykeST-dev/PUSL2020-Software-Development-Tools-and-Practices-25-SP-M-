using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlindMatch.Web.Controllers;

[Authorize(Roles = "SystemAdministrator")]
public class AuditController : Controller
{
    public IActionResult Index() => RedirectToPage("/Admin/AuditLog");
}
