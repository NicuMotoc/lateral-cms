using LateralCMS.Infrastructure.Persistence.EF;

namespace LateralCMS.Application.Commands;

public class DisableEntityCommand
{
    private readonly EfEntityRepository _repo;
    public DisableEntityCommand(EfEntityRepository repo) => _repo = repo;

    public async Task DisableAsync(string id)
    {
        await _repo.DisableAsync(id);
    }
}
