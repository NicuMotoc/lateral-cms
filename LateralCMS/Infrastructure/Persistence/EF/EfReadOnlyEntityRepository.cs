using LateralCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LateralCMS.Infrastructure.Persistence.EF;

public class EfReadOnlyEntityRepository(CmsDbContext db)
{
    private readonly CmsDbContext _db = db;

    public async Task<List<CmsEntity>> ListAsync(bool includeDisabled = false)
    {
        var query = _db.Entities
            .AsNoTracking()
            .Include(e => e.Versions)
            .AsQueryable();

        if (!includeDisabled)
            query = query.Where(e => !e.IsDisabled);
        
        return await query.ToListAsync();
    }
}
