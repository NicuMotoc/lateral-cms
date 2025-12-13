using LateralCMS.Application.Commands;
using LateralCMS.Application.DTOs;
using LateralCMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LateralCMS.WebApi.Controllers;

[ApiController]
[Route("cms/events")]
public class CmsEventsController(ProcessCmsEventsCommand command, SanitizationService sanitizer) : ControllerBase
{
    private readonly ProcessCmsEventsCommand _command = command;
    private readonly SanitizationService _sanitizer = sanitizer;

    [HttpPost]
    [Authorize(Roles = "CMS")]
    public async Task<IActionResult> Ingest([FromBody] CmsEventBatchDto batch)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _sanitizer.Sanitize(batch.Events);

        await _command.ProcessAsync(batch.Events);
        return Ok();
    }
}
