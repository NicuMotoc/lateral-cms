using LateralCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LateralCMS.Infrastructure.Persistence.EF;

public class CmsDbContext(DbContextOptions<CmsDbContext> options) : DbContext(options)
{
    public DbSet<CmsEntity> Entities => Set<CmsEntity>();
    public DbSet<CmsEntityVersion> EntityVersions => Set<CmsEntityVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CmsEntity>().HasKey(e => e.Id);
        modelBuilder.Entity<CmsEntity>().HasMany(e => e.Versions);
        base.OnModelCreating(modelBuilder);
    }
}
