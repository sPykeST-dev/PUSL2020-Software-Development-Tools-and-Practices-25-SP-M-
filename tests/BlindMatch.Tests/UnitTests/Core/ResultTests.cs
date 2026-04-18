using BlindMatch.Core.ValueObjects;

namespace BlindMatch.Tests.UnitTests.Core;

public class ResultTests
{
    [Fact]
    public void Success_IsSuccessTrue_ErrorNull()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_IsSuccessFalse_ErrorSet()
    {
        var result = Result.Failure("Something went wrong.");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Failure_PreservesErrorMessage()
    {
        const string message = "Specific failure reason.";
        var result = Result.Failure(message);

        result.Error.Should().Be(message);
    }
}
