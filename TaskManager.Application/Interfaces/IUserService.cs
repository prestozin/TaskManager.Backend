using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.DTOs.User.Response;

namespace TaskManager.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResultResponse<UserResponse>> GetUser();
        Task<ResultResponse<string>> UpdateUser(EditUserRequest request);
    }
}
