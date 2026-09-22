using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;

namespace TaskManager.Infra.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Core.Entities.TaskStatus> Status { get; set; }
    public DbSet<TaskPriority> Priorities { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(user => user.Id);

            entity.HasIndex(user => user.Email)
                .IsUnique();
        });

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.ToTable("Tasks");

            entity.HasKey(task => task.Id);

            entity.HasOne(task => task.User)
                .WithMany(user => user.Tasks)
                .HasForeignKey(task => task.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(task => task.TaskStatus)
                .WithMany()
                .HasForeignKey(task => task.StatusId);

            entity.HasOne(task => task.TaskPriority)
                .WithMany()
                .HasForeignKey(task => task.PriorityId);

            entity.Property(task => task.CreatedAt)
                .HasConversion(
                    date => date,
                    date => DateTime.SpecifyKind(date, DateTimeKind.Utc)
                );
        });

        modelBuilder.Entity<Core.Entities.TaskStatus>(entity =>
        {
            entity.ToTable("TaskStatus");

            entity.HasKey(status => status.Id);
        });

        modelBuilder.Entity<TaskPriority>(entity =>
        {
            entity.ToTable("TaskPriority");

            entity.HasKey(priority => priority.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}