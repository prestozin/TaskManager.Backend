using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.Validators;
using TaskManager.Application.Validators.Auth;

namespace TaskManager.Tests.Validators;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class AuthValidatorTests
{
    [Test]
    public void ShouldPassValidation_WhenCreateUserRequestIsValid()
    {
        // Arrange
        CreateUserValidator validator = new CreateUserValidator();

        CreateUserRequest request = new CreateUserRequest
        {
            Name = "Mateus",
            Email = "mateus@email.com",
            Password = "Password1!"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }
    [TestCase("", "mateus@email.com", "Password1!")]
    [TestCase("Ma", "mateus@email.com", "Password1!")]
    [TestCase("Mateus", "invalid-email", "Password1!")]
    [TestCase("Mateus", "mateus@email.com", "password")]
    public void ShouldFailValidation_WhenCreateUserRequestIsInvalid(string name, string email, string password)
    {
        // Arrange
        CreateUserValidator validator = new CreateUserValidator();

        CreateUserRequest request = new CreateUserRequest
        {
            Name = name,
            Email = email,
            Password = password
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldPassValidation_WhenLoginRequestIsValid()
    {
        // Arrange
        LoginValidator validator = new LoginValidator();

        LoginRequest request = new LoginRequest
        {
            Email = "mateus@email.com",
            Password = "any-password"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }
    [TestCase("", "password")]
    [TestCase("invalid-email", "password")]
    [TestCase("mateus@email.com", "")]
    public void ShouldFailValidation_WhenLoginRequestIsInvalid(string email, string password)
    {
        // Arrange
        LoginValidator validator = new LoginValidator();

        LoginRequest request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }
}
