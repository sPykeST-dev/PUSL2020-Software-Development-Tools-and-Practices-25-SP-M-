using BlindMatch.Core.ValueObjects;

namespace BlindMatch.Core.Interfaces.Services;

public interface IBlindMatchService
{
    Task<Result> ConfirmInterestAsync(int interestId, string supervisorId);
}
