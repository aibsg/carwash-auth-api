using carwash_auth_api.Models;

namespace carwash_auth_api.Repositories.Interfacecs;

public interface IAsyncRepository <TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetById(Guid id);
    Task<IEnumerable<TEntity>> GetAll();
    Task<Guid> Add(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
    Task Delete(Guid id);
}