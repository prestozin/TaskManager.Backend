using TaskManager.Application.DTOs;

namespace TaskManager.Tests.DTOs;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class ResultResponseTests
{
    [Test]
    public void ShouldCreateSuccessWithData_WhenDataIsProvided()
    {
        // Arrange

        // Act
        var result = ResultResponse<string>.Success("data", "success");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.EqualTo("success"));
        Assert.That(result.Data, Is.EqualTo("data"));
    }

    [Test]
    public void ShouldCreateSuccessWithoutData_WhenOnlyMessageIsProvided()
    {
        // Arrange

        // Act
        var result = ResultResponse<string>.Success("success");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Message, Is.EqualTo("success"));
        Assert.That(result.Data, Is.Null);
    }

    [Test]
    public void ShouldCreateFailureWithData_WhenDataIsProvided()
    {
        // Arrange

        // Act
        var result = ResultResponse<string>.Failure("data", "failure");

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("failure"));
        Assert.That(result.Data, Is.EqualTo("data"));
    }

    [Test]
    public void ShouldCreateFailureWithoutData_WhenOnlyMessageIsProvided()
    {
        // Arrange

        // Act
        var result = ResultResponse<string>.Failure("failure");

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Message, Is.EqualTo("failure"));
        Assert.That(result.Data, Is.Null);
    }
}
