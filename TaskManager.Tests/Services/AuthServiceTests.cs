using Microsoft.Extensions.Configuration;
using Moq;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Tests.Services;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        Dictionary<string, string?> settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "12345678901234567890123456789012",
            ["Jwt:Issuer"] = "TaskManager",
            ["Jwt:Audience"] = "TaskManager"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Test]
    public async Task ShouldReturnFailure_WhenRegisterEmailAlreadyExists()
    {
        // Arrange
        _userRepository
            .Setup(repository => repository.UserExistsAsync("user@email.com"))
            .ReturnsAsync(true);

        AuthService service = new AuthService(_userRepository.Object, _configuration);

        // Act
        var result = await service.CreateUserAsync(new CreateUserRequest
        {
            Name = "User",
            Email = "USER@EMAIL.COM",
            Password = "Password1!"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        _userRepository.Verify(repository => repository.AddUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task ShouldNormalizeAndHashPassword_WhenRegisterSucceeds()
    {
        // Arrange
        User? savedUser = null;

        _userRepository
            .Setup(repository => repository.UserExistsAsync("user@email.com"))
            .ReturnsAsync(false);

        _userRepository
            .Setup(repository => repository.AddUserAsync(It.IsAny<User>()))
            .Callback<User>(user => savedUser = user)
            .ReturnsAsync((User user) => user);

        AuthService service = new AuthService(_userRepository.Object, _configuration);

        // Act
        var result = await service.CreateUserAsync(new CreateUserRequest
        {
            Name = "  User  ",
            Email = "  USER@EMAIL.COM  ",
            Password = "Password1!"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser!.Name, Is.EqualTo("User"));
        Assert.That(savedUser.Email, Is.EqualTo("user@email.com"));
        Assert.That(savedUser.HashPassword, Is.Not.EqualTo("Password1!"));
        Assert.That(BCrypt.Net.BCrypt.Verify("Password1!", savedUser.HashPassword), Is.True);
    }

    [Test]
    public async Task ShouldReturnFailure_WhenLoginUserDoesNotExist()
    {
        // Arrange
        _userRepository
            .Setup(repository => repository.GetUserByEmailAsync("user@email.com"))
            .ReturnsAsync((User?)null);

        AuthService service = new AuthService(_userRepository.Object, _configuration);

        // Act
        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "USER@EMAIL.COM",
            Password = "Password1!"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
    }

    [Test]
    public async Task ShouldReturnFailure_WhenLoginPasswordIsInvalid()
    {
        // Arrange
        User user = new User
        {
            Email = "user@email.com",
            Name = "User",
            HashPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByEmailAsync("user@email.com"))
            .ReturnsAsync(user);

        AuthService service = new AuthService(_userRepository.Object, _configuration);

        // Act
        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@email.com",
            Password = "WrongPassword1!"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public async Task ShouldReturnToken_WhenLoginCredentialsAreValid()
    {
        // Arrange
        User user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@email.com",
            Name = "User",
            HashPassword = BCrypt.Net.BCrypt.HashPassword("Password1!")
        };

        _userRepository
            .Setup(repository => repository.GetUserByEmailAsync("user@email.com"))
            .ReturnsAsync(user);

        AuthService service = new AuthService(_userRepository.Object, _configuration);

        // Act
        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@email.com",
            Password = "Password1!"
        });

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(string.IsNullOrWhiteSpace(result.Data!.Token), Is.False);
    }
}
