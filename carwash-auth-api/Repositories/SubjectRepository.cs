using carwash_auth_api.Data;
using carwash_auth_api.Models;

namespace carwash_auth_api.Repositories;

public class SubjectRepository : BaseRepository<Subject>
{
    public  SubjectRepository(AppDbContext _dbContext) : base(_dbContext) { }
}