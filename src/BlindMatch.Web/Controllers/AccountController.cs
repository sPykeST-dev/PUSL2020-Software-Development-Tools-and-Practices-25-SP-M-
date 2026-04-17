using BlindMatch.Core.Common;
using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces;
using BlindMatch.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlindMatch.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser>   _userManager;
    private readonly IUserService                   _userService;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser>   userManager,
        IUserService                   userService)
    {
        _signInManager = signInManager;
        _userManager   = userManager;
        _userService   = userService;
    }
    

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return RedirectToRoleDashboard();

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty,
                "Your account is locked. Please try again later.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (result, _) = await _userService.CreateUserAsync(
            model.FirstName, model.LastName, model.Email, model.Password,
            Roles.Student, model.Programme, model.YearOfStudy);

        if (result.Succeeded)
        {
            await _signInManager.PasswordSignInAsync(
                model.Email, model.Password,
                isPersistent: false, lockoutOnFailure: false);
            return RedirectToRoleDashboard();
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }
    

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
    

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account",
                new { userId = user.Id, token }, Request.Scheme);

            // TODO: plug in a real email sender — for now logs to console
            Console.WriteLine($"[Password Reset Link] {resetLink}");
        }

        return View("ForgotPasswordConfirmation");
    }
    

    [HttpGet]
    public IActionResult ResetPassword(string userId, string token)
        => View(new ResetPasswordViewModel { UserId = userId, Token = token });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user is null) return RedirectToAction("ResetPasswordConfirmation");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation");

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [HttpGet] public IActionResult ResetPasswordConfirmation() => View();
    [HttpGet] public IActionResult ForgotPasswordConfirmation() => View();
    [HttpGet] public IActionResult AccessDenied() => View();
    

    private IActionResult RedirectToRoleDashboard()
    {
        if (User.IsInRole(Roles.Student))
            return RedirectToAction("Index", "Proposal");
        if (User.IsInRole(Roles.Supervisor))
            return RedirectToAction("Index", "SupervisorBrowse");
        if (User.IsInRole(Roles.ModuleLeader))
            return LocalRedirect("/module-leader/dashboard");
        if (User.IsInRole(Roles.SystemAdministrator))
            return RedirectToAction("Index", "UserManagement");

        return RedirectToAction("Index", "Home");
    }
}