namespace BlindMatch.Core.Enums;

public enum MatchStatus
{
    Pending = 0,    // Created after supervisor confirms — awaiting Module Leader approval
    Approved = 1,   // Module Leader approved (Member 6: was "Active" — now renamed)
    Rejected = 2,   // Module Leader rejected
    Cancelled = 3   // Voided for any reason
}