using Moq;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Tests.Services;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
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

    [Test]
    public async Task ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync((User?)null);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.GetUserAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
    }

    [Test]
    public async Task ShouldReturnUser_WhenUserExists()
    {
        // Arrange
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

        // Act
        var result = await service.GetUserAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data!.Name, Is.EqualTo("User"));
        Assert.That(result.Data.Email, Is.EqualTo("user@email.com"));
    }

    [Test]
    public async Task ShouldTrimAndPersistUser_WhenEditUserSucceeds()
    {
        // Arrange
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

        // Act
        var result = await service.EditUserAsync(new EditUserRequest
        {
            Name = "  New Name  ",
            Role = "Developer"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(user.Name, Is.EqualTo("New Name"));
        Assert.That(user.Role, Is.EqualTo("Developer"));
        _userRepository.Verify(repository => repository.EditUserByIdAsync(user), Times.Once);
    }

    [Test]
    public async Task ShouldReturnFailure_WhenDeletePasswordIsInvalid()
    {
        // Arrange
        User user = new User
        {
            Id = _userId,
            HashPassword = BCrypt.Net.BCrypt.HashPassword("Password1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.DeleteUserAsync(new DeleteUserRequest
        {
            Password = "WrongPassword1!"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        _userRepository.Verify(repository => repository.DeleteUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task ShouldDeleteUser_WhenPasswordIsValid()
    {
        // Arrange
        User user = new User
        {
            Id = _userId,
            HashPassword = BCrypt.Net.BCrypt.HashPassword("Password1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.DeleteUserAsync(new DeleteUserRequest
        {
            Password = "Password1!"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        _userRepository.Verify(repository => repository.DeleteUserAsync(user), Times.Once);
    }

    [Test]
    public async Task ShouldReturnFailure_WhenNewPasswordMatchesCurrentPassword()
    {
        // Arrange
        User user = new User
        {
            Id = _userId,
            HashPassword = BCrypt.Net.BCrypt.HashPassword("Password1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.ChangePasswordAsync(new ChangeUserPasswordRequest
        {
            OldPassword = "Password1!",
            NewPassword = "Password1!"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        _userRepository.Verify(repository => repository.EditUserByIdAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task ShouldUpdatePasswordHash_WhenNewPasswordIsValid()
    {
        // Arrange
        User user = new User
        {
            Id = _userId,
            HashPassword = BCrypt.Net.BCrypt.HashPassword("OldPassword1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByIdAsync(_userId))
            .ReturnsAsync(user);

        UserService service = new UserService(_userRepository.Object, _currentUserContext.Object);

        // Act
        var result = await service.ChangePasswordAsync(new ChangeUserPasswordRequest
        {
            OldPassword = "OldPassword1!",
            NewPassword = "NewPassword1!"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(BCrypt.Net.BCrypt.Verify("NewPassword1!", user.HashPassword), Is.True);
        _userRepository.Verify(repository => repository.EditUserByIdAsync(user), Times.Once);
    }
}
