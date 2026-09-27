using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.Validators;
using TaskManager.Application.Validators.Auth;

namespace TaskManager.Tests.Validators;

public class AuthValidatorTests
{
    [Fact]
    public void ShouldPassValidation_WhenCreateUserRequestIsValid()
    {
        CreateUserValidator validator = new CreateUserValidator();

        CreateUserRequest request = new CreateUserRequest
        {
            Name = "Mateus",
            Email = "mateus@email.com",
            Password = "Password1!"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "mateus@email.com", "Password1!")]
    [InlineData("Ma", "mateus@email.com", "Password1!")]
    [InlineData("Mateus", "invalid-email", "Password1!")]
    [InlineData("Mateus", "mateus@email.com", "password")]
    public void ShouldFailValidation_WhenCreateUserRequestIsInvalid(string name, string email, string password)
    {
        CreateUserValidator validator = new CreateUserValidator();

        CreateUserRequest request = new CreateUserRequest
        {
            Name = name,
            Email = email,
            Password = password
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldPassValidation_WhenLoginRequestIsValid()
    {
        LoginValidator validator = new LoginValidator();

        LoginRequest request = new LoginRequest
        {
            Email = "mateus@email.com",
            Password = "any-password"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("invalid-email", "password")]
    [InlineData("mateus@email.com", "")]
    public void ShouldFailValidation_WhenLoginRequestIsInvalid(string email, string password)
    {
        LoginValidator validator = new LoginValidator();

        LoginRequest request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
