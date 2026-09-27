using TaskManager.Application.DTOs;

namespace TaskManager.Tests.DTOs;

public class ResultResponseTests
{
    [Fact]
    public void ShouldCreateSuccessWithData_WhenDataIsProvided()
    {
        var result = ResultResponse<string>.Success("data", "success");

        Assert.True(result.IsSuccess);
        Assert.Equal("success", result.Message);
        Assert.Equal("data", result.Data);
    }

    [Fact]
    public void ShouldCreateSuccessWithoutData_WhenOnlyMessageIsProvided()
    {
        var result = ResultResponse<string>.Success("success");

        Assert.True(result.IsSuccess);
        Assert.Equal("success", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public void ShouldCreateFailureWithData_WhenDataIsProvided()
    {
        var result = ResultResponse<string>.Failure("data", "failure");

        Assert.False(result.IsSuccess);
        Assert.Equal("failure", result.Message);
        Assert.Equal("data", result.Data);
    }

    [Fact]
    public void ShouldCreateFailureWithoutData_WhenOnlyMessageIsProvided()
    {
        var result = ResultResponse<string>.Failure("failure");

        Assert.False(result.IsSuccess);
        Assert.Equal("failure", result.Message);
        Assert.Null(result.Data);
    }
}
