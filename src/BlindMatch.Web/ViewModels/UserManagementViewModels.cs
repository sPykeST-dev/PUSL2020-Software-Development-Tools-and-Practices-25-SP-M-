using System.ComponentModel.DataAnnotations;

namespace BlindMatch.Web.ViewModels;

public class UserListViewModel
{
    public IEnumerable<UserRowViewModel> Users { get; set; } = [];
}

public class UserRowViewModel
{
    public string Id       { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email    { get; set; } = string.Empty;
    public string Role     { get; set; } = string.Empty;
    public bool   IsActive { get; set; }
}

public class CreateUserViewModel
{
    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = string.Empty;
}

public class ChangeRoleViewModel
{
    [Required] public string UserId  { get; set; } = string.Empty;
    [Required] public string NewRole { get; set; } = string.Empty;
}