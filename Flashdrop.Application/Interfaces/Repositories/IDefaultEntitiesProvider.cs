using Flashdrop.Data.Entities;

namespace Flashdrop.Application.Interfaces.Repositories;

public interface IDefaultEntitiesProvider
{
    Task<User> GetDefaultSellerAsync(CancellationToken cancellationToken = default);
    Task<User> GetDefaultAdminAsync(CancellationToken cancellationToken = default);
}
