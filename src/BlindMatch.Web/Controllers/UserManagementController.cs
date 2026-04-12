using BlindMatch.Core.Common;
using BlindMatch.Core.Interfaces;
using BlindMatch.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlindMatch.Web.Controllers;

[Authorize(Policy = Policies.AdminOnly)]
public class UserManagementController : Controller
{
    private readonly IUserService _userService;

    public UserManagementController(IUserService userService)
        => _userService = userService;
    

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();

        var rows = new List<UserRowViewModel>();
        foreach (var u in users)
        {
            var role = await _userService.GetRoleAsync(u) ?? "—";
            rows.Add(new UserRowViewModel
            {
                Id       = u.Id,
                FullName = u.FullName,
                Email    = u.Email ?? string.Empty,
                Role     = role,
                IsActive = u.IsActive
            });
        }

        return View(new UserListViewModel { Users = rows });
    }
    

    [HttpGet]
    public IActionResult Create() => View(new CreateUserViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // RULE: Admins must never create Student accounts
        if (model.Role == Roles.Student)
        {
            ModelState.AddModelError(nameof(model.Role),
                "Student accounts cannot be created here. Students self-register.");
            return View(model);
        }

        var (result, _) = await _userService.CreateUserAsync(
            model.FirstName, model.LastName, model.Email, model.Password, model.Role);

        if (result.Succeeded)
        {
            TempData["Success"] = $"Account created for {model.Email}.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
    {
        if (model.NewRole == Roles.Student)
        {
            TempData["Error"] = "Cannot assign the Student role from this panel.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userService.ChangeRoleAsync(model.UserId, model.NewRole);
        TempData[result.Succeeded ? "Success" : "Error"] =
            result.Succeeded ? "Role updated." : result.Errors.First().Description;

        return RedirectToAction(nameof(Index));
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(string userId)
    {
        var result = await _userService.DeactivateUserAsync(userId);
        TempData[result.Succeeded ? "Success" : "Error"] =
            result.Succeeded ? "User deactivated." : result.Errors.First().Description;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reactivate(string userId)
    {
        var result = await _userService.ReactivateUserAsync(userId);
        TempData[result.Succeeded ? "Success" : "Error"] =
            result.Succeeded ? "User reactivated." : result.Errors.First().Description;
        return RedirectToAction(nameof(Index));
    }
}