using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.DTOs.User.Response;
using TaskManager.Application.Interfaces;

namespace TaskManager.Tests.Controllers;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class UserControllerTests
{
    private readonly Mock<IUserService> _userService = new Mock<IUserService>();

    [Test]
    public async Task ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        _userService
            .Setup(service => service.GetUserAsync())
            .ReturnsAsync(ResultResponse<UserResponse>.Success(new UserResponse()));

        UserController controller = new UserController(_userService.Object);

        // Act
        var result = await controller.GetUserAsync();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _userService
            .Setup(service => service.GetUserAsync())
            .ReturnsAsync(ResultResponse<UserResponse>.Failure("Not found"));

        UserController controller = new UserController(_userService.Object);

        // Act
        var result = await controller.GetUserAsync();

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenEditUserSucceeds()
    {
        // Arrange
        EditUserRequest request = new EditUserRequest();

        _userService
            .Setup(service => service.EditUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Updated"));

        UserController controller = new UserController(_userService.Object);

        // Act
        var result = await controller.EditUserAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnNotFound_WhenEditUserFails()
    {
        // Arrange
        EditUserRequest request = new EditUserRequest();

        _userService
            .Setup(service => service.EditUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Not found"));

        UserController controller = new UserController(_userService.Object);

        // Act
        var result = await controller.EditUserAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenDeleteUserSucceeds()
    {
        // Arrange
        DeleteUserRequest request = new DeleteUserRequest { Password = "Password1!" };

        _userService
            .Setup(service => service.DeleteUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Deleted"));

        UserController controller = new UserController(_userService.Object);

        // Act
        var result = await controller.DeleteUserAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnBadRequest_WhenDeleteUserFails()
    {
        // Arrange
        DeleteUserRequest request = new DeleteUserRequest { Password = "Wrong" };

        _userService
            .Setup(service => service.DeleteUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Failed"));

        UserController controller = new UserController(_userService.Object);

        // Act
        var result = await controller.DeleteUserAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task ShouldReturnOk_WhenChangePasswordSucceeds()
    {
        // Arrange
        ChangeUserPasswordRequest request = new ChangeUserPasswordRequest();

        _userService
            .Setup(service => service.ChangePasswordAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Updated"));

        UserController controller = new UserController(_userService.Object);

        // Act
        var result = await controller.ChangePasswordAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ShouldReturnBadRequest_WhenChangePasswordFails()
    {
        // Arrange
        ChangeUserPasswordRequest request = new ChangeUserPasswordRequest();

        _userService
            .Setup(service => service.ChangePasswordAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Failed"));

        UserController controller = new UserController(_userService.Object);

        // Act
        var result = await controller.ChangePasswordAsync(request);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
}
