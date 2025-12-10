using LateralCMS.Infrastructure.Persistence.EF;

namespace LateralCMS.Application.Commands;

public class DisableEntityCommand(EfEntityRepository repo)
{
    private readonly EfEntityRepository _repo = repo;

    public async Task DisableAsync(string id)
    {
        await _repo.DisableAsync(id);
    }
}
