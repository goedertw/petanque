using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers;

[ApiController]
[Route("api/scores")]
public class ScoreController(IScoreService service) : Controller
{
    [HttpGet("{id}")]
    public ActionResult<SpelResponseContract> Get([FromRoute] int id)
    {
        var spel = service.GetById(id);
        if (spel is null) return NotFound();
        return Ok(spel);
    }

    [HttpPost]
    public ActionResult<SpelResponseContract> Create(
        [FromBody] SpelRequestContract request)
    {
        var created = service.Create(request);
        return CreatedAtAction(nameof(Get), new { id = created.SpelId }, created);
    }
}