namespace BlindMatch.Core.Enums;

public enum MatchStatus
{
    Pending = 0,    // Created after supervisor confirms — awaiting Module Leader approval
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}
