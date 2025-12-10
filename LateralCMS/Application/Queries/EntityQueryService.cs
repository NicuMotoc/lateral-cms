using LateralCMS.Infrastructure.Persistence.EF;
using LateralCMS.Domain.Entities;

namespace LateralCMS.Application.Queries;

public class EntityQueryService
{
    private readonly EfEntityRepository _repo;
    public EntityQueryService(EfEntityRepository repo) => _repo = repo;

    public async Task<List<CmsEntity>> ListAsync(bool isAdmin)
    {
        return await _repo.ListAsync(includeDisabled: isAdmin);
    }
}
