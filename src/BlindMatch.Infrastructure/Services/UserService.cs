using BlindMatch.Core.Common;
using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BlindMatch.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole>    _roleManager;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole>    roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<(IdentityResult Result, ApplicationUser? User)> CreateUserAsync(
        string firstName, string lastName, string email, string password, string role,
        string? programme = null, int? yearOfStudy = null, string? department = null)
    {
        ApplicationUser user = role switch
        {
            Roles.Supervisor => new Supervisor
            {
                FirstName       = firstName,
                LastName        = lastName,
                Email           = email,
                UserName        = email,
                IsActive        = true,
                EmailConfirmed  = true,
                Department      = department ?? string.Empty,
                MaxProjects     = 3,
                CurrentProjects = 0
            },
            Roles.Student => new Student
            {
                FirstName   = firstName,
                LastName    = lastName,
                Email       = email,
                UserName    = email,
                Programme   = programme,
                YearOfStudy = yearOfStudy,
                IsActive    = true
            },
            _ => new ApplicationUser
            {
                FirstName   = firstName,
                LastName    = lastName,
                Email       = email,
                UserName    = email,
                IsActive    = true,
                EmailConfirmed = true
            }
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) return (result, null);

        var roleResult = await AssignRoleAsync(user, role);
        if (!roleResult.Succeeded) return (roleResult, null);

        return (IdentityResult.Success, user);
    }

    public async Task<IdentityResult> AssignRoleAsync(ApplicationUser user, string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
            return IdentityResult.Failed(new IdentityError
                { Description = $"Role '{role}' does not exist." });

        return await _userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> ChangeRoleAsync(string userId, string newRole)
    {
        var user = await GetUserByIdAsync(userId);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded) return removeResult;

        return await AssignRoleAsync(user, newRole);
    }

    public async Task<IdentityResult> DeactivateUserAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        user.IsActive = false;
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ReactivateUserAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        user.IsActive = true;
        return await _userManager.UpdateAsync(user);
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        => await _userManager.FindByIdAsync(userId);

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        => _userManager.Users.ToList();

    public async Task<string?> GetRoleAsync(ApplicationUser user)
        => (await _userManager.GetRolesAsync(user)).FirstOrDefault();
}