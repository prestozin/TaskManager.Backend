using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<bool> UserExistsAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(Guid userId);
        Task EditUserByIdAsync(User user);
        Task DeleteUserAsync(User user);
    }
}
