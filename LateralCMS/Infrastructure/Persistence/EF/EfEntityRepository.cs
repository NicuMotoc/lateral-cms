using LateralCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LateralCMS.Infrastructure.Persistence.EF;

public class EfEntityRepository(CmsDbContext db)
{
    private readonly CmsDbContext _db = db;

    public async Task<CmsEntity?> GetByIdAsync(int id) =>
        await _db.Entities
            .AsNoTracking()
            .Include(e => e.Versions)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<CmsEntity?> GetByIdAndVersionAsync(int id, int version)
    {
        var entity = await _db.Entities
            .AsNoTracking()
            .Include(e => e.Versions.Where(v => v.Version == version))
            .FirstOrDefaultAsync(e => e.Id == id);
        return entity;
    }

    public async Task<CmsEntityVersion?> GetVersionAsync(int id, int version)
    {
        return await _db.EntityVersions.AsNoTracking().FirstOrDefaultAsync(e => e.CmsEntityId == id && e.Version == version);
    }

    public async Task<List<CmsEntity>> ListAsync(bool includeDisabled = false)
    {
        var query = _db.Entities
            .AsNoTracking()
            .Include(e => e.Versions)
            .AsQueryable();
        if (!includeDisabled)
            query = query.Where(e => !e.IsDisabled && e.IsPublished);
        return await query.ToListAsync();
    }

    public async Task AddOrUpdateAsync(CmsEntity entity, string? payload = null)
    {
        var existing = await _db.Entities.Include(e => e.Versions).FirstOrDefaultAsync(e => e.Id == entity.Id);
        if (existing == null)
        {
            _db.Entities.Add(entity);
        }
        else
        {
            // Only add new version if payload is provided (for update)
            if (payload != null)
            {
                var newVersion = existing.Versions.Count != 0 ? existing.Versions.Max(v => v.Version) + 1 : 1;
                existing.Versions.Add(new CmsEntityVersion
                {
                    Version = newVersion,
                    Timestamp = DateTime.UtcNow,
                    Payload = payload,
                    IsUnpublished = true
                });
                existing.LatestVersion = newVersion;
            }
            // Update IsPublished, IsDisabled, LatestVersion from entity
            existing.IsPublished = entity.IsPublished;
            existing.IsDisabled = entity.IsDisabled;
            existing.LatestVersion = entity.LatestVersion;
            // Update version states
            foreach (var v in existing.Versions)
            {
                var updated = entity.Versions.FirstOrDefault(ev => ev.Version == v.Version);
                if (updated != null)
                    v.IsUnpublished = updated.IsUnpublished;
            }
        }
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Entities.Include(e => e.Versions).FirstOrDefaultAsync(e => e.Id == id);
        if (entity != null)
        {
            _db.Entities.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }

    public async Task PublishAsync(int id, int version)
    {
        var entity = await _db.Entities.Include(e => e.Versions).FirstOrDefaultAsync(e => e.Id == id);
        if (entity != null)
        {
            foreach (var v in entity.Versions)
                v.IsUnpublished = v.Version != version;
            entity.IsPublished = true;
            entity.IsDisabled = false;
            entity.LatestVersion = version;
            await _db.SaveChangesAsync();
        }
    }

    public async Task UnpublishAsync(int id, int version)
    {
        var entity = await _db.Entities.Include(e => e.Versions).FirstOrDefaultAsync(e => e.Id == id);
        if (entity != null)
        {
            var versionToUnpublish = entity.Versions.FirstOrDefault(e => e.Version == version);
            if (versionToUnpublish != null)
                versionToUnpublish.IsUnpublished = true;
            await _db.SaveChangesAsync();
        }
    }

    public async Task DisableAsync(int id)
    {
        var entity = await _db.Entities.FirstOrDefaultAsync(e => e.Id == id);
        if (entity != null)
        {
            entity.IsDisabled = true;
            entity.IsPublished = false;
            await _db.SaveChangesAsync();
        }
    }
}
