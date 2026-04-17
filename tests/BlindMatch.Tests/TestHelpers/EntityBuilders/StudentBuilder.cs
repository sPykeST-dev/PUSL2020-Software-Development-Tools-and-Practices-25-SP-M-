using BlindMatch.Core.Entities;

namespace BlindMatch.Tests.TestHelpers.EntityBuilders;

public class StudentBuilder
{
    private readonly Student _student = new()
    {
        Id                 = Guid.NewGuid().ToString(),
        UserName           = "student@test.com",
        Email              = "student@test.com",
        NormalizedEmail    = "STUDENT@TEST.COM",
        NormalizedUserName = "STUDENT@TEST.COM",
        FirstName          = "Test",
        LastName           = "Student",
        IsActive           = true,
        Programme          = "BSc Computer Science",
        YearOfStudy        = 3,
        SecurityStamp      = Guid.NewGuid().ToString()
    };

    public StudentBuilder WithId(string id)
    {
        _student.Id = id;
        return this;
    }

    public StudentBuilder WithEmail(string email)
    {
        _student.Email             = email;
        _student.UserName          = email;
        _student.NormalizedEmail   = email.ToUpperInvariant();
        _student.NormalizedUserName = email.ToUpperInvariant();
        return this;
    }

    public StudentBuilder Inactive()
    {
        _student.IsActive = false;
        return this;
    }

    public Student Build() => _student;
}
