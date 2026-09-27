using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infra.Data;
using TaskManager.Infra.Data.Repositories;

namespace TaskManager.Tests.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task ShouldAddAndFindUser_WhenUserIsCreated()
    {
        await using ApplicationDbContext context = CreateContext();
        UserRepository repository = new UserRepository(context);

        User user = new User
        {
            Name = "Mateus",
            Email = "mateus@email.com",
            HashPassword = "hash"
        };

        User created = await repository.AddUserAsync(user);
        User? byId = await repository.GetUserByIdAsync(user.Id);
        User? byEmail = await repository.GetUserByEmailAsync(user.Email);

        Assert.Same(user, created);
        Assert.Equal(user.Id, byId!.Id);
        Assert.Equal(user.Id, byEmail!.Id);
    }

    [Fact]
    public async Task ShouldReturnTrue_WhenEmailAlreadyExists()
    {
        await using ApplicationDbContext context = CreateContext();

        context.Users.Add(new User
        {
            Name = "Mateus",
            Email = "mateus@email.com",
            HashPassword = "hash"
        });

        await context.SaveChangesAsync();

        UserRepository repository = new UserRepository(context);

        Assert.True(await repository.UserExistsAsync("mateus@email.com"));
        Assert.False(await repository.UserExistsAsync("other@email.com"));
    }

    [Fact]
    public async Task ShouldPersistChanges_WhenUserIsEdited()
    {
        await using ApplicationDbContext context = CreateContext();

        User user = new User
        {
            Name = "Old Name",
            Email = "mateus@email.com",
            HashPassword = "hash"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        UserRepository repository = new UserRepository(context);

        user.Name = "New Name";
        await repository.EditUserByIdAsync(user);

        User? saved = await context.Users.SingleOrDefaultAsync(item => item.Id == user.Id);

        Assert.Equal("New Name", saved!.Name);
    }

    [Fact]
    public async Task ShouldRemoveUser_WhenDeleteUserIsCalled()
    {
        await using ApplicationDbContext context = CreateContext();

        User user = new User
        {
            Name = "Mateus",
            Email = "mateus@email.com",
            HashPassword = "hash"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        UserRepository repository = new UserRepository(context);

        await repository.DeleteUserAsync(user);

        Assert.False(await context.Users.AnyAsync(item => item.Id == user.Id));
    }

    private static ApplicationDbContext CreateContext()
    {
        DbContextOptions<ApplicationDbContext> options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new ApplicationDbContext(options);
    }
}
