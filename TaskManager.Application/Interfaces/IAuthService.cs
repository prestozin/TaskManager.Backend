using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.DTOs.Auth.Response;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Interfaces;

public interface IAuthService
{
    Task<ResultDto<CreateUserRequest>> RegisterAsync(CreateUserRequest userRegisterDto);
    Task<ResultDto<LoginResponse>> LoginAsync(UserLoginResponse userLoginDto);
}
