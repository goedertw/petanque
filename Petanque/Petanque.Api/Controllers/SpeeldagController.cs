using Microsoft.AspNetCore.Mvc;
using Petanque.Services;
using Petanque.Contracts;

namespace Petanque.Api.Controllers;

[ApiController]
[Route("api/speeldag")]
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
}