using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Auth.Request;
using TaskManager.Application.DTOs.Auth.Response;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Interfaces;

public interface IAuthService
{
    Task<ResultResponse<string>> CreateUserAsync(CreateUserRequest request);
    Task<ResultResponse<LoginResponse>> LoginAsync(LoginRequest userLoginDto);
}
