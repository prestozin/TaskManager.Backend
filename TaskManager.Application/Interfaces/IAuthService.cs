using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.DTOs.Auth.Response;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Interfaces;

public interface IAuthService
{
    Task<ResultResponse<CreateUserRequest>> CreateUserAsync(CreateUserRequest userRegisterDto);
    Task<ResultResponse<LoginResponse>> LoginAsync(LoginRequest userLoginDto);
}
