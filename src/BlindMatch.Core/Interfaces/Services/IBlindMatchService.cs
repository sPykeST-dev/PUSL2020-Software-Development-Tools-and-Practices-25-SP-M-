using BlindMatch.Core.ValueObjects;

namespace BlindMatch.Core.Interfaces.Services;

public interface IBlindMatchService
{
    // Runs the full confirm-interest transaction: marks interest confirmed,
    // creates IdentityReveal, creates Match, updates proposal status,
    // increments supervisor project count. Everything succeeds or nothing saves.
    Task<Result> ConfirmInterestAsync(int interestId, string supervisorId);
}