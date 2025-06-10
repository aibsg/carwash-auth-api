using carwash_auth_api.Data;
using carwash_auth_api.Models;
using Microsoft.EntityFrameworkCore;

namespace carwash_auth_api.Repositories;

public class UserRepository : BaseRepository<User>
{
    public UserRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<User?> GetByEmail(string email) =>
        await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    
    public async Task<User?> GetByRefreshToken (string refToken) =>
        await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refToken);
    
}