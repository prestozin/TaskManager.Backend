using Microsoft.Extensions.Configuration;
using Moq;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Tests.Services;

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

    [Fact]
    public async Task ShouldReturnFailure_WhenRegisterEmailAlreadyExists()
    {
        _userRepository
            .Setup(repository => repository.UserExistsAsync("user@email.com"))
            .ReturnsAsync(true);

        AuthService service = new AuthService(_userRepository.Object, _configuration);

        var result = await service.CreateUserAsync(new CreateUserRequest
        {
            Name = "User",
            Email = "USER@EMAIL.COM",
            Password = "Password1!"
        });

        Assert.False(result.IsSuccess);
        _userRepository.Verify(repository => repository.AddUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ShouldNormalizeAndHashPassword_WhenRegisterSucceeds()
    {
        User? savedUser = null;

        _userRepository
            .Setup(repository => repository.UserExistsAsync("user@email.com"))
            .ReturnsAsync(false);

        _userRepository
            .Setup(repository => repository.AddUserAsync(It.IsAny<User>()))
            .Callback<User>(user => savedUser = user)
            .ReturnsAsync((User user) => user);

        AuthService service = new AuthService(_userRepository.Object, _configuration);

        var result = await service.CreateUserAsync(new CreateUserRequest
        {
            Name = "  User  ",
            Email = "  USER@EMAIL.COM  ",
            Password = "Password1!"
        });

        Assert.True(result.IsSuccess);
        Assert.NotNull(savedUser);
        Assert.Equal("User", savedUser!.Name);
        Assert.Equal("user@email.com", savedUser.Email);
        Assert.NotEqual("Password1!", savedUser.HashPassword);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password1!", savedUser.HashPassword));
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenLoginUserDoesNotExist()
    {
        _userRepository
            .Setup(repository => repository.GetUserByEmailAsync("user@email.com"))
            .ReturnsAsync((User?)null);

        AuthService service = new AuthService(_userRepository.Object, _configuration);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "USER@EMAIL.COM",
            Password = "Password1!"
        });

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenLoginPasswordIsInvalid()
    {
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

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@email.com",
            Password = "WrongPassword1!"
        });

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ShouldReturnToken_WhenLoginCredentialsAreValid()
    {
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

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@email.com",
            Password = "Password1!"
        });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.False(string.IsNullOrWhiteSpace(result.Data!.Token));
    }
}
