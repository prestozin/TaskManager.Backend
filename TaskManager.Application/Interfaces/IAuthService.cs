using TaskManager.Application.DTOs;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Interfaces;

public interface IAuthService
{
    Task<ResultDto<CreateUserDto>> RegisterAsync(CreateUserDto userRegisterDto);
    Task<ResultDto<LoginResponseDto>> LoginAsync(UserLoginDto userLoginDto);
}
