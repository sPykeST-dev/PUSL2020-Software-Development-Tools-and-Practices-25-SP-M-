using BlindMatch.Core.Entities;

namespace BlindMatch.Tests.TestHelpers.EntityBuilders;

public class SupervisorBuilder
{
    private readonly Supervisor _supervisor = new()
    {
        Id                 = Guid.NewGuid().ToString(),
        UserName           = "supervisor@test.com",
        Email              = "supervisor@test.com",
        NormalizedEmail    = "SUPERVISOR@TEST.COM",
        NormalizedUserName = "SUPERVISOR@TEST.COM",
        FirstName          = "Test",
        LastName           = "Supervisor",
        Department         = "Computer Science",
        MaxProjects        = 3,
        CurrentProjects    = 0,
        IsActive           = true,
        SecurityStamp      = Guid.NewGuid().ToString()
    };

    public SupervisorBuilder WithId(string id)
    {
        _supervisor.Id = id;
        return this;
    }

    public SupervisorBuilder WithMaxProjects(int max)
    {
        _supervisor.MaxProjects = max;
        return this;
    }

    public SupervisorBuilder WithCurrentProjects(int current)
    {
        _supervisor.CurrentProjects = current;
        return this;
    }

    public SupervisorBuilder AtCapacity()
    {
        _supervisor.MaxProjects     = 2;
        _supervisor.CurrentProjects = 2;
        return this;
    }

    public SupervisorBuilder WithEmail(string email)
    {
        _supervisor.Email             = email;
        _supervisor.UserName          = email;
        _supervisor.NormalizedEmail   = email.ToUpperInvariant();
        _supervisor.NormalizedUserName = email.ToUpperInvariant();
        return this;
    }

    public SupervisorBuilder WithDepartment(string dept)
    {
        _supervisor.Department = dept;
        return this;
    }

    public Supervisor Build() => _supervisor;
}
