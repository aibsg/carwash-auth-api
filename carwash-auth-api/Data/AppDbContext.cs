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
}