using LateralCMS.Application.Commands;
using LateralCMS.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LateralCMS.WebApi.Controllers;

[ApiController]
[Route("cms/events")]
public class CmsEventsController(ProcessCmsEventsCommand command) : ControllerBase
{
    private readonly ProcessCmsEventsCommand _command = command;

    [HttpPost]
    [Authorize(Policy = "CmsOnly")]
    public async Task<IActionResult> Ingest([FromBody] List<CmsEventDto> events)
    {
        await _command.ProcessAsync(events);
        return Ok();
    }
}
