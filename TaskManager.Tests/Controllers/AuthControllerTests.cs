using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.DTOs.Auth.Response;
using TaskManager.Application.Interfaces;

namespace TaskManager.Tests.Controllers;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authService = new Mock<IAuthService>();

    [Test]
    public async Task ShouldReturnOk_WhenRegisterSucceeds()
    {
        // Arrange
        CreateUserRequest request = new CreateUserRequest();

        _authService
            .Setup(service => service.CreateUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Created"));

        AuthController controller = new AuthController(_authService.Object);

        // Act
        var result = await controller.CreateUserAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnConflict_WhenRegisterFails()
    {
        // Arrange
        CreateUserRequest request = new CreateUserRequest();

        _authService
            .Setup(service => service.CreateUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Conflict"));

        AuthController controller = new AuthController(_authService.Object);

        // Act
        var result = await controller.CreateUserAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<ConflictObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenLoginSucceeds()
    {
        // Arrange
        LoginRequest request = new LoginRequest();

        _authService
            .Setup(service => service.LoginAsync(request))
            .ReturnsAsync(ResultResponse<LoginResponse>.Success(new LoginResponse { Token = "token" }));

        AuthController controller = new AuthController(_authService.Object);

        // Act
        var result = await controller.LoginAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnUnauthorized_WhenLoginFails()
    {
        // Arrange
        LoginRequest request = new LoginRequest();

        _authService
            .Setup(service => service.LoginAsync(request))
            .ReturnsAsync(ResultResponse<LoginResponse>.Failure("Invalid credentials"));

        AuthController controller = new AuthController(_authService.Object);

        // Act
        var result = await controller.LoginAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
    }
}
