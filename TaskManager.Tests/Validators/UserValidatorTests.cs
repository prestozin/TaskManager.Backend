using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.Validators.User;

namespace TaskManager.Tests.Validators;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class UserValidatorTests
{
    [Test]
    public void ShouldPassValidation_WhenEditUserRequestIsValid()
    {
        // Arrange
        EditUserValidator validator = new EditUserValidator();

        EditUserRequest request = new EditUserRequest
        {
            Name = "Mateus",
            Role = "Developer",
            Area = "Software",
            About = "About"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }
    [TestCase("")]
    [TestCase("ab")]
    public void ShouldFailValidation_WhenEditUserNameIsInvalid(string name)
    {
        // Arrange
        EditUserValidator validator = new EditUserValidator();

        EditUserRequest request = new EditUserRequest
        {
            Name = name
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldFailValidation_WhenEditUserOptionalFieldExceedsMaximumLength()
    {
        // Arrange
        EditUserValidator validator = new EditUserValidator();

        EditUserRequest request = new EditUserRequest
        {
            Name = "Mateus",
            Role = new string('a', 101)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldPassValidation_WhenDeleteUserPasswordIsValid()
    {
        // Arrange
        DeleteUserValidator validator = new DeleteUserValidator();

        DeleteUserRequest request = new DeleteUserRequest
        {
            Password = "password"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ShouldFailValidation_WhenDeleteUserPasswordIsEmpty()
    {
        // Arrange
        DeleteUserValidator validator = new DeleteUserValidator();

        DeleteUserRequest request = new DeleteUserRequest
        {
            Password = ""
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ShouldPassValidation_WhenChangePasswordRequestIsValid()
    {
        // Arrange
        ChangeUserPasswordValidator validator = new ChangeUserPasswordValidator();

        ChangeUserPasswordRequest request = new ChangeUserPasswordRequest
        {
            OldPassword = "OldPassword1!",
            NewPassword = "NewPassword1!"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }
    [TestCase("", "NewPassword1!")]
    [TestCase("OldPassword1!", "weak")]
    public void ShouldFailValidation_WhenChangePasswordRequestIsInvalid(string oldPassword, string newPassword)
    {
        // Arrange
        ChangeUserPasswordValidator validator = new ChangeUserPasswordValidator();

        ChangeUserPasswordRequest request = new ChangeUserPasswordRequest
        {
            OldPassword = oldPassword,
            NewPassword = newPassword
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
    }
}
