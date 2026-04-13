namespace BlindMatch.Core.ValueObjects;

public class Result
{
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
}
