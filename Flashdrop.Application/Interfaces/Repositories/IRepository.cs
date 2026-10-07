namespace Flashdrop.Application.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<TEntity> GetAll();
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Delete(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
