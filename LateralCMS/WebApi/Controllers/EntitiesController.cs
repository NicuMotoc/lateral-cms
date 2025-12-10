using LateralCMS.Application.Commands;
using LateralCMS.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LateralCMS.WebApi.Controllers;

[ApiController]
[Route("entities")]
public class EntitiesController(EntityQueryService query, DisableEntityCommand disable) : ControllerBase
{
    private readonly EntityQueryService _query = query;
    private readonly DisableEntityCommand _disable = disable;

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> List()
    {
        var isAdmin = User.IsInRole("admin");
        var entities = await _query.ListAsync(isAdmin);
        return Ok(entities);
    }

    [HttpPost("{id}/disable")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Disable(string id)
    {
        await _disable.DisableAsync(id);
        return Ok();
    }
}
