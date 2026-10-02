using Flashdrop.Application.Common.Exceptions;
using Flashdrop.Application.Interfaces.Repositories;

namespace Flashdrop.Application.Common.Extensions;

public static class RepositoryExtensions
{
    public static async Task<TEntity> GetByIdOrThrowAsync<TEntity>(
        this IRepository<TEntity> repository,
        Guid id,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return entity ?? throw new NotFoundException(typeof(TEntity).Name, id);
    }
}
