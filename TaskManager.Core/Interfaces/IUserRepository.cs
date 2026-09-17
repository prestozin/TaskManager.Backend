using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(Guid userId);
    }
}
