
using Mapster;
using Microsoft.Extensions.Configuration;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.User;
using TaskManager.Application.Interfaces;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResultDto<UserResponseDto>> GetUserById(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
            return ResultDto<UserResponseDto>.Failure("Usuário não encontrado");

        UserResponseDto userDto = user.Adapt<UserResponseDto>();

        return ResultDto<UserResponseDto>.Success(userDto);
    }
}

