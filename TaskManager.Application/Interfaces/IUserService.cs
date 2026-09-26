using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.DTOs.User.Response;

namespace TaskManager.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResultResponse<UserResponse>> GetUserAsync();
        Task<ResultResponse<string>> EditUserAsync(EditUserRequest request);
        Task<ResultResponse<string>> DeleteUserAsync(DeleteUserRequest request);
        Task<ResultResponse<string>> ChangePasswordAsync(ChangeUserPasswordRequest request);
    }
}
