using LateralCMS.Application.DTOs;
using LateralCMS.Domain.Entities;
using LateralCMS.Infrastructure.Persistence.EF;

namespace LateralCMS.Application.Commands;

public class ProcessCmsEventsCommand(EfEntityRepository repo, ILogger<ProcessCmsEventsCommand> logger)
{
    private readonly EfEntityRepository _repo = repo;
    private readonly ILogger<ProcessCmsEventsCommand> _logger = logger;

    public async Task ProcessAsync(IEnumerable<CmsEventDto> events)
    {
        foreach (var evt in events)
        {
            try
            {
                switch (evt.Type.ToLowerInvariant())
                {
                    case "add":
                        await HandleAdd(evt);
                        break;
                    case "update":
                        await HandleUpdate(evt);
                        break;
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

    private async Task HandleAdd(CmsEventDto evt)
    {
        var entity = new CmsEntity
        {
            LatestVersion = 1,
            IsPublished = false,
            IsDisabled = false,
            Versions =
            [
                new CmsEntityVersion
                {
                    Version = 1,
                    Timestamp = evt.Timestamp,
                    Payload = evt.Payload!,
                    IsUnpublished = true
                }
            ]
        };

        await _repo.AddOrUpdateAsync(entity);
    }

    private async Task HandleUpdate(CmsEventDto evt)
    {
        var entity = await _repo.GetByIdAsync(evt.Id);

        if (entity == null) return;

        var previousVersion = entity.LatestVersion;
        entity.LatestVersion++;
        await _repo.UnpublishAsync(entity.Id, previousVersion);
        await _repo.AddOrUpdateAsync(entity, evt.Payload);
    }

    private async Task HandlePublish(CmsEventDto evt)
    {
        if (evt.Id == 0 || evt.Version is null)
            return;

        await _repo.PublishAsync(evt.Id, evt.Version.Value);
    }

    private async Task HandleUnpublish(CmsEventDto evt)
    {
        if (evt.Id == 0 || evt.Version is null)
            return;

        await _repo.UnpublishAsync(evt.Id, evt.Version.Value);
    }
}
