using LateralCMS.Domain.Entities;
using LateralCMS.Infrastructure.Persistence.EF;

namespace LateralCMS.Application.Queries;

public class EntityQueryService(EfEntityRepository repo)
{
    private readonly EfEntityRepository _repo = repo;

    public async Task<List<CmsEntity>> ListAsync(bool isAdmin)
    {
        return await _repo.ListAsync(includeDisabled: isAdmin);
    }
}
