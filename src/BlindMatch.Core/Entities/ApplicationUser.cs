namespace BlindMatch.Core.Entities;

public class ApplicationUser
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public bool   IsActive  { get; set; } = true;

    public string? Programme   { get; set; }
    public int?    YearOfStudy { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}