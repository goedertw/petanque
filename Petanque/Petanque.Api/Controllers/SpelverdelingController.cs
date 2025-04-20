using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using static Petanque.Contracts.Responses.SpelverdelingSpellenResponseContract;

namespace Petanque.Api.Controllers;
[ApiController]
[Route("api/spelverdelingen")]
public class SpelverdelingController(ISpelverdelingService service, IAanwezigheidService Aservice) : Controller
{
    [HttpGet("{id}")]
    public ActionResult<PlayerResponseContract> Get([FromRoute] int id)
    {
        var spelverdeling = service.GetById(id);
        if (spelverdeling is null) return NotFound();
        return Ok(spelverdeling);
    }
    [HttpPost("{id}")]
    public ActionResult<IEnumerable<SpelverdelingResponseContract>> MaakVerdeling([FromRoute] int id)
    {
        var aanwezigheden = Aservice.GetAanwezighedenOpSpeeldag(id);
        

        if (aanwezigheden == null || !aanwezigheden.Any())
            return BadRequest("Geen aanwezige spelers gevonden.");

        return Ok(service.MaakVerdeling(aanwezigheden));
    }
}