using BlindMatch.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlindMatch.Infrastructure.Services;

public class ActiveUserFilter : IAsyncActionFilter
{
    private readonly UserManager<ApplicationUser>   _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ActiveUserFilter(
        UserManager<ApplicationUser>   userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(context.HttpContext.User);

            if (user is null || !user.IsActive)
            {
                await _signInManager.SignOutAsync();
                context.Result = new RedirectToActionResult(
                    "Login", "Account",
                    new { returnUrl = context.HttpContext.Request.Path });
                return;
            }
        }

        await next();
    }
}