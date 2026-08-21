using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;

namespace TaskManager.Infra.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Core.Entities.TaskStatus> Status { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.HashPassword)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(t => t.Description)
                .HasMaxLength(500);

            entity.HasOne(t => t.User)
                .WithMany(t => t.Tasks)
                .HasForeignKey(t => t.UserId);

            entity.HasOne(t => t.TaskStatus)
                .WithMany()
                .HasForeignKey(t => t.StatusId);

            entity.HasOne(t => t.TaskPriority)
                .WithMany()
                .HasForeignKey(t => t.PriorityId);
        });

        modelBuilder.Entity<Core.Entities.TaskStatus>(entity =>
        {
            entity.ToTable("TaskStatus");

            entity.HasKey(ts => ts.Id);

            entity.Property(ts => ts.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<TaskPriority>(entity =>
        {
            entity.ToTable("TaskPriority");

            entity.HasKey(ts => ts.Id);

            entity.Property(ts => ts.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        base.OnModelCreating(modelBuilder);
    }
}

