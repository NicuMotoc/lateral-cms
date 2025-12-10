using LateralCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LateralCMS.Infrastructure.Persistence.EF;

public class EfEntityRepository
{
    private readonly CmsDbContext _db;
    public EfEntityRepository(CmsDbContext db) => _db = db;

    public async Task<CmsEntity?> GetByIdAsync(string id) =>
        await _db.Entities.Include(e => e.Versions).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<List<CmsEntity>> ListAsync(bool includeDisabled = false)
    {
        var query = _db.Entities.Include(e => e.Versions).AsQueryable();
        if (!includeDisabled)
            query = query.Where(e => !e.IsDisabledByAdmin && e.IsPublished);
        return await query.ToListAsync();
    }

    public async Task AddOrUpdateAsync(CmsEntity entity)
    {
        var existing = await _db.Entities.Include(e => e.Versions).FirstOrDefaultAsync(e => e.Id == entity.Id);
        if (existing == null)
            _db.Entities.Add(entity);
        else
        {
            existing.LatestVersion = entity.LatestVersion;
            existing.IsPublished = entity.IsPublished;
            existing.Versions = entity.Versions;
        }
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var entity = await _db.Entities.FirstOrDefaultAsync(e => e.Id == id);
        if (entity != null)
        {
            _db.Entities.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DisableAsync(string id)
    {
        var entity = await _db.Entities.FirstOrDefaultAsync(e => e.Id == id);
        if (entity != null)
        {
            entity.IsDisabledByAdmin = true;
            await _db.SaveChangesAsync();
        }
    }
}
