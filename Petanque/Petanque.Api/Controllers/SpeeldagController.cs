using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers;

[ApiController]
[Route("api/speeldagen")]
public class SpeeldagController(ISpeeldagService service) : Controller
{
    [HttpPost]
    public ActionResult<SpeeldagResponseContract> CreateSpeeldag([FromBody] SpeeldagRequestContract request)
    {
        if (request == null)
        {
            return BadRequest("Request is null");
        }

        var createdSpeeldag = service.Create(request);
        return CreatedAtAction(nameof(CreateSpeeldag), new { id = createdSpeeldag.SpeeldagId }, createdSpeeldag);
    }
    [HttpGet("{id}")]
    public ActionResult<SpeeldagResponseContract> Get([FromRoute] int id)
    {
        var speeldag = service.GetById(id);
        if (speeldag is null) return NotFound();
        return Ok(speeldag);
    }
}