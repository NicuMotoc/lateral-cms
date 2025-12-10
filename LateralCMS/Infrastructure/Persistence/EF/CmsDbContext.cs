using LateralCMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LateralCMS.Infrastructure.Persistence.EF;

public class CmsDbContext : DbContext
{
    public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options) { }

    public DbSet<CmsEntity> Entities => Set<CmsEntity>();
    public DbSet<CmsEntityVersion> EntityVersions => Set<CmsEntityVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CmsEntity>().HasKey(e => e.Id);
        modelBuilder.Entity<CmsEntity>().OwnsMany(e => e.Versions);
        base.OnModelCreating(modelBuilder);
    }
}
