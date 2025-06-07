using Microsoft.EntityFrameworkCore;
using carwash_auth_api.Models;

namespace carwash_auth_api.Data;

public class AppDbContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Subject> Subjects { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Уточняем связь: User -> Subject (один к одному)
        modelBuilder.Entity<User>()
            .HasOne(u => u.SubjectInfo)
            .WithMany() // или .WithOne() если Subject не используется в других отношениях
            .HasForeignKey(u => u.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Названия таблиц и схем (если нужно)
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Subject>().ToTable("subject");

        base.OnModelCreating(modelBuilder);
    }
}