namespace BlindMatch.Core.Common;

public static class Roles
{
    public const string Student              = "Student";
    public const string Supervisor           = "Supervisor";
    public const string ModuleLeader         = "ModuleLeader";
    public const string SystemAdministrator  = "SystemAdministrator";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Student, Supervisor, ModuleLeader, SystemAdministrator
    };
}

public static class Policies
{
    public const string StudentOnly      = "StudentOnly";
    public const string SupervisorOnly   = "SupervisorOnly";
    public const string ModuleLeaderOnly = "ModuleLeaderOnly";
    public const string AdminOnly        = "AdminOnly";
}

public static class AppClaimTypes
{
    public const string Programme   = "Programme";
    public const string YearOfStudy = "YearOfStudy";
    public const string FullName    = "FullName";
}