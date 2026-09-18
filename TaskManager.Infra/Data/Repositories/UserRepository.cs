
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infra.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users
            .SingleOrDefaultAsync(u => u.Id == userId);
    }

    public async Task EditUserByIdAsync(User user)
    {
        _context.Update(user);
        await _context.SaveChangesAsync();
    }

}
