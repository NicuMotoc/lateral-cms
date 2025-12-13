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
        var isAdmin = User.IsInRole("Admin");
        var entities = await _query.ListAsync(isAdmin);

        if (entities.Count == 0)
        {
            return NoContent();
        }

        return Ok(entities);
    }

    [HttpPost("{id}/disable")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Disable(int id)
    {
        await _disable.DisableAsync(id);
        return Ok();
    }
}
