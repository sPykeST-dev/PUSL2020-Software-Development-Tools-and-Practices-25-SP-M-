using Microsoft.AspNetCore.Identity;

namespace BlindMatch.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public bool   IsActive  { get; set; } = true;

    public string? Programme   { get; set; }
    public int?    YearOfStudy { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
