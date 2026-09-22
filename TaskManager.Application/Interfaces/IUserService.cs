using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.DTOs.User.Response;
using TaskManager.Application.Validators.User;

namespace TaskManager.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResultResponse<UserResponse>> GetUserAsync();
        Task<ResultResponse<string>> EditUserAsync(EditUserRequest request);
        Task<ResultResponse<string>> DeleteUserAsync(string userPassword);
        Task<ResultResponse<string>> ChangePasswordAsync(ChangeUserPasswordRequest request);
    }
}
