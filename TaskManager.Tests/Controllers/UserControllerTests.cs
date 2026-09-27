using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.DTOs.User.Response;
using TaskManager.Application.Interfaces;

namespace TaskManager.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userService = new Mock<IUserService>();

    [Fact]
    public async Task ShouldReturnOk_WhenUserExists()
    {
        _userService
            .Setup(service => service.GetUserAsync())
            .ReturnsAsync(ResultResponse<UserResponse>.Success(new UserResponse()));

        UserController controller = new UserController(_userService.Object);

        var result = await controller.GetUserAsync();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        _userService
            .Setup(service => service.GetUserAsync())
            .ReturnsAsync(ResultResponse<UserResponse>.Failure("Not found"));

        UserController controller = new UserController(_userService.Object);

        var result = await controller.GetUserAsync();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenEditUserSucceeds()
    {
        EditUserRequest request = new EditUserRequest();

        _userService
            .Setup(service => service.EditUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Updated"));

        UserController controller = new UserController(_userService.Object);

        var result = await controller.EditUserAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenEditUserFails()
    {
        EditUserRequest request = new EditUserRequest();

        _userService
            .Setup(service => service.EditUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Not found"));

        UserController controller = new UserController(_userService.Object);

        var result = await controller.EditUserAsync(request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenDeleteUserSucceeds()
    {
        DeleteUserRequest request = new DeleteUserRequest { Password = "Password1!" };

        _userService
            .Setup(service => service.DeleteUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Deleted"));

        UserController controller = new UserController(_userService.Object);

        var result = await controller.DeleteUserAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenDeleteUserFails()
    {
        DeleteUserRequest request = new DeleteUserRequest { Password = "Wrong" };

        _userService
            .Setup(service => service.DeleteUserAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Failed"));

        UserController controller = new UserController(_userService.Object);

        var result = await controller.DeleteUserAsync(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenChangePasswordSucceeds()
    {
        ChangeUserPasswordRequest request = new ChangeUserPasswordRequest();

        _userService
            .Setup(service => service.ChangePasswordAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Updated"));

        UserController controller = new UserController(_userService.Object);

        var result = await controller.ChangePasswordAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenChangePasswordFails()
    {
        ChangeUserPasswordRequest request = new ChangeUserPasswordRequest();

        _userService
            .Setup(service => service.ChangePasswordAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Failed"));

        UserController controller = new UserController(_userService.Object);

        var result = await controller.ChangePasswordAsync(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
