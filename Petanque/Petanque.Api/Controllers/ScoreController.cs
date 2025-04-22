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

    [HttpPut("{id}")]
    public IActionResult UpdateScore(int id, [FromBody] SpelRequestContract request) {
        try {
            service.UpdateScore(id, request.ScoreA, request.ScoreB);
            return NoContent();
        } catch (Exception ex) {
            return NotFound(ex.Message);
        }
    }
}