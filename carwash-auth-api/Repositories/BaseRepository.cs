using carwash_auth_api.Data;
using carwash_auth_api.Models;
using carwash_auth_api.Repositories.Interfacecs;
using Microsoft.EntityFrameworkCore;

namespace carwash_auth_api.Repositories;

public abstract class  BaseRepository<TEntity> : IAsyncRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly AppDbContext _dbContext;

    public BaseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<TEntity?> GetById(Guid id)
    {
        return await _dbContext.FindAsync<TEntity>(id);
    }

    public async Task<IEnumerable<TEntity>> GetAll()
    {
        return await _dbContext.Set<TEntity>().ToListAsync();
    }

    public async Task<Guid> Add(TEntity entity)
    {
        await _dbContext.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Update(TEntity entity)
    {
        _dbContext.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(TEntity entity)
    {
        _dbContext.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Set<TEntity>().FindAsync(id);
        if (entity != null)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Сущность с ID {id} не найдена");
        }
    }
}