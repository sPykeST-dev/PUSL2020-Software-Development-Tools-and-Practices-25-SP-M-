using BlindMatch.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlindMatch.Core.Interfaces;

public interface IUserService
{
    Task<(IdentityResult Result, ApplicationUser? User)> CreateUserAsync(
        string firstName, string lastName, string email, string password, string role,
        string? programme = null, int? yearOfStudy = null);

    Task<IdentityResult> AssignRoleAsync(ApplicationUser user, string role);
    Task<IdentityResult> ChangeRoleAsync(string userId, string newRole);
    Task<IdentityResult> DeactivateUserAsync(string userId);
    Task<IdentityResult> ReactivateUserAsync(string userId);

    Task<ApplicationUser?>             GetUserByIdAsync(string userId);
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<string?>                      GetRoleAsync(ApplicationUser user);
}