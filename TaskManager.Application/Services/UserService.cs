
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
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResultDto<UserResponse>> GetUser(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
            return ResultDto<UserResponse>.Failure(string.Format(Messages.FIELD_NOT_FOUND, "Usuário"));

        UserResponse userDto = user.Adapt<UserResponse>();

        return ResultDto<UserResponse>.Success(userDto);
    }

    public async Task<ResultDto<string>> EditUser(EditUserRequest request, Guid userId)
    {
        EditUserValidator validator = new EditUserValidator();
        await validator.ValidateAndThrowAsync(request);

        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
            return ResultDto<string>.Failure(string.Format(Messages.FIELD_NOT_FOUND, "Usuário"));

        request.Adapt(user);

        await _userRepository.UpdateUserByIdAsync(user);

        return ResultDto<string>.Success("Usuário atualizado com sucesso.");
    }
}

