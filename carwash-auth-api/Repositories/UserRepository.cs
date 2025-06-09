using carwash_auth_api.Data;
using carwash_auth_api.Models;
using carwash_auth_api.Repositories.Interfacecs;

namespace carwash_auth_api.Repositories;

public class UserRepository : BaseRepository<User>
{
    public UserRepository(AppDbContext dbContext) : base(dbContext) { }
}