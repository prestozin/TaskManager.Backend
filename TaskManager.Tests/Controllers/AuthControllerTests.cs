using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.DTOs.Auth.Response;
using TaskManager.Application.Interfaces;

namespace TaskManager.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authService = new Mock<IAuthService>();

    [Fact]
    public async Task ShouldReturnOk_WhenRegisterSucceeds()
    {
        CreateUserRequest request = new CreateUserRequest();

        _authService
            .Setup(service => service.CreateUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Created"));

        AuthController controller = new AuthController(_authService.Object);

        var result = await controller.CreateUserAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenRegisterFails()
    {
        CreateUserRequest request = new CreateUserRequest();

        _authService
            .Setup(service => service.CreateUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Conflict"));

        AuthController controller = new AuthController(_authService.Object);

        var result = await controller.CreateUserAsync(request);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenLoginSucceeds()
    {
        LoginRequest request = new LoginRequest();

        _authService
            .Setup(service => service.LoginAsync(request))
            .ReturnsAsync(ResultResponse<LoginResponse>.Success(new LoginResponse { Token = "token" }));

        AuthController controller = new AuthController(_authService.Object);

        var result = await controller.LoginAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenLoginFails()
    {
        LoginRequest request = new LoginRequest();

        _authService
            .Setup(service => service.LoginAsync(request))
            .ReturnsAsync(ResultResponse<LoginResponse>.Failure("Invalid credentials"));

        AuthController controller = new AuthController(_authService.Object);

        var result = await controller.LoginAsync(request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
