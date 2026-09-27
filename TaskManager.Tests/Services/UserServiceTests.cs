using Moq;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Tests.Services;

public class UserServiceTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
    private readonly Mock<ICurrentUserContext> _currentUserContext = new Mock<ICurrentUserContext>();

    public UserServiceTests()
    {
        _currentUserContext
            .Setup(context => context.UserId)
            .Returns(_userId);
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenUserDoesNotExist()
    {
        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync((User?)null);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        var result = await service.GetUserAsync();

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ShouldReturnUser_WhenUserExists()
    {
        User user = new User
        {
            Id = _userId,
            Name = "User",
            Email = "user@email.com"
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        var result = await service.GetUserAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal("User", result.Data!.Name);
        Assert.Equal("user@email.com", result.Data.Email);
    }

    [Fact]
    public async Task ShouldTrimAndPersistUser_WhenEditUserSucceeds()
    {
        User user = new User
        {
            Id = _userId,
            Name = "Old Name",
            Email = "user@email.com"
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        var result = await service.EditUserAsync(new EditUserRequest
        {
            Name = "  New Name  ",
            Role = "Developer"
        });

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", user.Name);
        Assert.Equal("Developer", user.Role);
        _userRepository.Verify(repository => repository.EditUserByIdAsync(user), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenDeletePasswordIsInvalid()
    {
        User user = new User
        {
            Id = _userId,
            HashPassword = BCrypt.Net.BCrypt.HashPassword("Password1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        var result = await service.DeleteUserAsync(new DeleteUserRequest
        {
            Password = "WrongPassword1!"
        });

        Assert.False(result.IsSuccess);
        _userRepository.Verify(repository => repository.DeleteUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ShouldDeleteUser_WhenPasswordIsValid()
    {
        User user = new User
        {
            Id = _userId,
            HashPassword = BCrypt.Net.BCrypt.HashPassword("Password1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        var result = await service.DeleteUserAsync(new DeleteUserRequest
        {
            Password = "Password1!"
        });

        Assert.True(result.IsSuccess);
        _userRepository.Verify(repository => repository.DeleteUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenNewPasswordMatchesCurrentPassword()
    {
        User user = new User
        {
            Id = _userId,
            HashPassword = BCrypt.Net.BCrypt.HashPassword("Password1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        var result = await service.ChangePasswordAsync(new ChangeUserPasswordRequest
        {
            OldPassword = "Password1!",
            NewPassword = "Password1!"
        });

        Assert.False(result.IsSuccess);
        _userRepository.Verify(repository => repository.EditUserByIdAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ShouldUpdatePasswordHash_WhenNewPasswordIsValid()
    {
        User user = new User
        {
            Id = _userId,
            HashPassword = BCrypt.Net.BCrypt.HashPassword("OldPassword1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        var result = await service.ChangePasswordAsync(new ChangeUserPasswordRequest
        {
            OldPassword = "OldPassword1!",
            NewPassword = "NewPassword1!"
        });

        Assert.True(result.IsSuccess);
        Assert.True(BCrypt.Net.BCrypt.Verify("NewPassword1!", user.HashPassword));
        _userRepository.Verify(repository => repository.EditUserByIdAsync(user), Times.Once);
    }
}
