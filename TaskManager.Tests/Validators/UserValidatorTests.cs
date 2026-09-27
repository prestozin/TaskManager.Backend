using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.Validators.User;

namespace TaskManager.Tests.Validators;

public class UserValidatorTests
{
    [Fact]
    public void ShouldPassValidation_WhenEditUserRequestIsValid()
    {
        EditUserValidator validator = new EditUserValidator();

        EditUserRequest request = new EditUserRequest
        {
            Name = "Mateus",
            Role = "Developer",
            Area = "Software",
            About = "About"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void ShouldFailValidation_WhenEditUserNameIsInvalid(string name)
    {
        EditUserValidator validator = new EditUserValidator();

        EditUserRequest request = new EditUserRequest
        {
            Name = name
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldFailValidation_WhenEditUserOptionalFieldExceedsMaximumLength()
    {
        EditUserValidator validator = new EditUserValidator();

        EditUserRequest request = new EditUserRequest
        {
            Name = "Mateus",
            Role = new string('a', 101)
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldPassValidation_WhenDeleteUserPasswordIsValid()
    {
        DeleteUserValidator validator = new DeleteUserValidator();

        DeleteUserRequest request = new DeleteUserRequest
        {
            Password = "password"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ShouldFailValidation_WhenDeleteUserPasswordIsEmpty()
    {
        DeleteUserValidator validator = new DeleteUserValidator();

        DeleteUserRequest request = new DeleteUserRequest
        {
            Password = ""
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ShouldPassValidation_WhenChangePasswordRequestIsValid()
    {
        ChangeUserPasswordValidator validator = new ChangeUserPasswordValidator();

        ChangeUserPasswordRequest request = new ChangeUserPasswordRequest
        {
            OldPassword = "OldPassword1!",
            NewPassword = "NewPassword1!"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "NewPassword1!")]
    [InlineData("OldPassword1!", "weak")]
    public void ShouldFailValidation_WhenChangePasswordRequestIsInvalid(string oldPassword, string newPassword)
    {
        ChangeUserPasswordValidator validator = new ChangeUserPasswordValidator();

        ChangeUserPasswordRequest request = new ChangeUserPasswordRequest
        {
            OldPassword = oldPassword,
            NewPassword = newPassword
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
