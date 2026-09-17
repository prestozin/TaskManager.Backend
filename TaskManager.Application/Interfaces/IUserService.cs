using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.User;

namespace TaskManager.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResultDto<UserResponseDto>> GetUserById(Guid userId);
    }
}
