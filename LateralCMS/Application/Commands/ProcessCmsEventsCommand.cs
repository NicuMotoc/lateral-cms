using LateralCMS.Application.DTOs;
using LateralCMS.Domain.Entities;
using LateralCMS.Infrastructure.Persistence.EF;
using Microsoft.Extensions.Logging;

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
                        await HandleDelete(evt);
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
            Id = evt.Id,
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
        await _repo.AddOrUpdateAsync(entity, evt.Payload);
    }

    private async Task HandleUpdate(CmsEventDto evt)
    {
        var entity = await _repo.GetByIdAsync(evt.Id);
        if (entity == null) return;
       
        await _repo.AddOrUpdateAsync(entity, evt.Payload);
    }

    private async Task HandlePublish(CmsEventDto evt)
    {
        if (evt.Id == 0 || evt.Version is null)
            return;
        var entity = await _repo.GetByIdAsync(evt.Id);
        if (entity == null) return;
        foreach (var v in entity.Versions)
            v.IsUnpublished = v.Version != evt.Version.Value;
        entity.IsPublished = true;
        entity.IsDisabled = false;
        entity.LatestVersion = evt.Version.Value;
        await _repo.PublishAsync(evt.Id, evt.Version.Value);
        await _repo.AddOrUpdateAsync(entity);
    }

    private async Task HandleUnpublish(CmsEventDto evt)
    {
        if (evt.Id == 0 || evt.Version is null)
            return;
        var entity = await _repo.GetByIdAsync(evt.Id);
        if (entity == null) return;
        var versionToUnpublish = entity.Versions.FirstOrDefault(v => v.Version == evt.Version.Value);
        if (versionToUnpublish != null)
            versionToUnpublish.IsUnpublished = true;
        var previousPublished = entity.Versions
            .Where(v => !v.IsUnpublished && v.Version != evt.Version.Value)
            .OrderByDescending(v => v.Version)
            .FirstOrDefault();
        if (previousPublished != null)
        {
            foreach (var v in entity.Versions)
                v.IsUnpublished = v.Version != previousPublished.Version;
            entity.IsPublished = true;
            entity.IsDisabled = false;
            entity.LatestVersion = previousPublished.Version;
            await _repo.PublishAsync(entity.Id, previousPublished.Version);
        }
        else
        {
            entity.IsPublished = false;
            entity.IsDisabled = true;
            await _repo.DisableAsync(entity.Id);
        }
        await _repo.AddOrUpdateAsync(entity);
    }

    private async Task HandleDelete(CmsEventDto evt)
    {
        if (evt.Id == 0)
            return;
        await _repo.DeleteAsync(evt.Id);
    }
}
