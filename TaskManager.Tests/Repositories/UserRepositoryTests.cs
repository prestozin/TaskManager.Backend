using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infra.Data;
using TaskManager.Infra.Data.Repositories;

namespace TaskManager.Tests.Repositories;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class UserRepositoryTests
{
    [Test]
    public async Task ShouldAddAndFindUser_WhenUserIsCreated()
    {
        // Arrange
        await using ApplicationDbContext context = CreateContext();
        UserRepository repository = new UserRepository(context);

        User user = new User
        {
            Name = "Mateus",
            Email = "mateus@email.com",
            HashPassword = "hash"
        };

        // Act
        User created = await repository.AddUserAsync(user);
        User? byId = await repository.GetUserByIdAsync(user.Id);
        User? byEmail = await repository.GetUserByEmailAsync(user.Email);

        // Assert
        Assert.That(created, Is.SameAs(user));
        Assert.That(byId!.Id, Is.EqualTo(user.Id));
        Assert.That(byEmail!.Id, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task ShouldReturnTrue_WhenEmailAlreadyExists()
    {
        // Arrange
        await using ApplicationDbContext context = CreateContext();

        context.Users.Add(new User
        {
            Name = "Mateus",
            Email = "mateus@email.com",
            HashPassword = "hash"
        });

        await context.SaveChangesAsync();

        UserRepository repository = new UserRepository(context);

        // Act
        bool exists = await repository.UserExistsAsync("mateus@email.com");
        bool otherExists = await repository.UserExistsAsync("other@email.com");

        // Assert
        Assert.That(exists, Is.True);
        Assert.That(otherExists, Is.False);
    }

    [Test]
    public async Task ShouldPersistChanges_WhenUserIsEdited()
    {
        // Arrange
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
        // Act
        await repository.EditUserByIdAsync(user);

        User? saved = await context.Users.SingleOrDefaultAsync(item => item.Id == user.Id);

        // Assert
        Assert.That(saved!.Name, Is.EqualTo("New Name"));
    }

    [Test]
    public async Task ShouldRemoveUser_WhenDeleteUserIsCalled()
    {
        // Arrange
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

        // Act
        await repository.DeleteUserAsync(user);

        // Assert
        Assert.That(await context.Users.AnyAsync(item => item.Id == user.Id), Is.False);
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
