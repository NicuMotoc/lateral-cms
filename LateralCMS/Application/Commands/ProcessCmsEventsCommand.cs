using LateralCMS.Application.DTOs;
using LateralCMS.Domain.Entities;
using LateralCMS.Infrastructure.Persistence.EF;
using Microsoft.Extensions.Logging;

namespace LateralCMS.Application.Commands;

public class ProcessCmsEventsCommand
{
    private readonly EfEntityRepository _repo;
    private readonly ILogger<ProcessCmsEventsCommand> _logger;
    public ProcessCmsEventsCommand(EfEntityRepository repo, ILogger<ProcessCmsEventsCommand> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task ProcessAsync(IEnumerable<CmsEventDto> events)
    {
        foreach (var evt in events)
        {
            try
            {
                switch (evt.Type.ToLowerInvariant())
                {
                    case "publish":
                        await HandlePublish(evt);
                        break;
                    case "unpublish":
                        await HandleUnpublish(evt);
                        break;
                    case "delete":
                        await _repo.DeleteAsync(evt.Id);
                        break;
                }
                _logger.LogInformation("Processed event {Type} for {Id}", evt.Type, evt.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process event {Type} for {Id}", evt.Type, evt.Id);
            }
        }
    }

    private async Task HandlePublish(CmsEventDto evt)
    {
        var entity = await _repo.GetByIdAsync(evt.Id) ?? new CmsEntity { Id = evt.Id };
        var version = new CmsEntityVersion
        {
            Version = evt.Version ?? 1,
            Timestamp = evt.Timestamp,
            Payload = evt.Payload!,
            IsUnpublished = false
        };
        entity.LatestVersion = version.Version;
        entity.IsPublished = true;
        entity.IsDisabledByAdmin = false;
        entity.Versions.RemoveAll(v => v.Version == version.Version);
        entity.Versions.Add(version);
        await _repo.AddOrUpdateAsync(entity);
    }

    private async Task HandleUnpublish(CmsEventDto evt)
    {
        var entity = await _repo.GetByIdAsync(evt.Id);
        if (entity == null) return;
        var version = entity.Versions.FirstOrDefault(v => v.Version == evt.Version);
        if (version != null)
        {
            version.IsUnpublished = true;
            entity.IsPublished = false;
            entity.LatestVersion = version.Version;
            await _repo.AddOrUpdateAsync(entity);
        }
        else
        {
            // Corner case: unpublish for a version not present
            entity.IsPublished = false;
            await _repo.AddOrUpdateAsync(entity);
        }
    }
}
