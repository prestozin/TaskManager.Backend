using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.DTOs.User.Response;

namespace TaskManager.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResultDto<UserResponse>> GetUser(Guid userId);
        Task<ResultDto<string>> EditUser(EditUserRequest request, Guid userId);
    }
}
