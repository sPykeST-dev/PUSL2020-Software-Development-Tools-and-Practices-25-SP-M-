namespace BlindMatch.Core.Entities;

public class Student : ApplicationUser
{
    public Proposal? Proposal { get; set; }
}
