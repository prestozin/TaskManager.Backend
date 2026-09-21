
using FluentValidation;
using Mapster;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.DTOs.User.Response;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Validators.User;
using TaskManager.Core.Constants;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserContext _currentUserContext;

    private Guid UserId => _currentUserContext.UserId;
    public UserService(IUserRepository userRepository, ICurrentUserContext currentUserContext)
    {
        _userRepository = userRepository;
        _currentUserContext = currentUserContext;
    }

    public async Task<ResultResponse<UserResponse>> GetUserAsync()
    {
        var user = await _userRepository.GetUserByIdAsync(UserId);

        if (user == null)
            return ResultResponse<UserResponse>.Failure(string.Format(Messages.FIELD_NOT_FOUND, "Usuário"));

        UserResponse userDto = user.Adapt<UserResponse>();

        return ResultResponse<UserResponse>.Success(userDto);
    }

    public async Task<ResultResponse<string>> EditUserAsync(EditUserRequest request)
    {
        EditUserValidator validator = new EditUserValidator();
        await validator.ValidateAndThrowAsync(request);

        var user = await _userRepository.GetUserByIdAsync(UserId);

        if (user == null)
            return ResultResponse<string>.Failure(string.Format(Messages.FIELD_NOT_FOUND, "Usuário"));

        request.Adapt(user);

        await _userRepository.EditUserByIdAsync(user);

        return ResultResponse<string>.Success(Messages.USER_UPDATED_SUCCESSFULLY);
    }

    public async Task<ResultResponse<string>> DeleteUserAsync(string userPassword)
    {
        if (string.IsNullOrWhiteSpace(userPassword))
            return ResultResponse<string>.Failure(string.Format(Messages.FIELD_REQUIRED, "Senha"));

        var user = await _userRepository.GetUserByIdAsync(UserId);

        if (user == null)
            return ResultResponse<string>.Failure(string.Format(Messages.FIELD_NOT_FOUND, "Usuário"));

        bool validPassword = BCrypt.Net.BCrypt.Verify(userPassword, user.HashPassword);

        if (!validPassword)
            return ResultResponse<string>.Failure(Messages.PASSWORD_INVALID);

        await _userRepository.DeleteUserAsync(user);
            return ResultResponse<string>.Success(Messages.USER_DELETED_SUCCESSFULLY);
    }
}

