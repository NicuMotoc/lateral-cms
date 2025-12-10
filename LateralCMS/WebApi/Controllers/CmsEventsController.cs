using LateralCMS.Application.Commands;
using LateralCMS.Application.DTOs;
using LateralCMS.WebApi.Services;
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
    public async Task<IActionResult> Ingest([FromBody] List<CmsEventDto> events)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _sanitizer.Sanitize(events);

        await _command.ProcessAsync(events);
        return Ok();
    }
}
