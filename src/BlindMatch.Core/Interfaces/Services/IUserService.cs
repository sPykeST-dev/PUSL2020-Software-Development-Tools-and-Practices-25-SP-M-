// Stub interface - will be implemented by Member 2 with proper ASP.NET Identity types
using BlindMatch.Core.Entities;

namespace BlindMatch.Core.Interfaces;

public interface IUserService
{
    // Methods will be implemented by Member 2
    Task<object> CreateUserAsync(string firstName, string lastName, string email, string password, string role, string? programme = null, int? yearOfStudy = null);
    Task<object> AssignRoleAsync(ApplicationUser user, string role);
    Task<object> ChangeRoleAsync(string userId, string newRole);
    Task<object> DeactivateUserAsync(string userId);
    Task<object> ReactivateUserAsync(string userId);
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<string?> GetRoleAsync(ApplicationUser user);
}