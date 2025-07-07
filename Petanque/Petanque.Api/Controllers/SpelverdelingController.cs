using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers;
[ApiController]
[Route("api/spelverdelingen")]
public class SpelverdelingController(ISpelverdelingService service, IAanwezigheidService Aservice) : Controller
{
    [HttpGet("{id}")]
    public ActionResult<IEnumerable<SpelverdelingResponseContract>> Get([FromRoute] int id)

    {
        var spelverdeling = service.GetById(id);
        if (spelverdeling is null) return NotFound();
        return Ok(spelverdeling);
    }
    [HttpPost("{id}")]
    public ActionResult<IEnumerable<SpelverdelingResponseContract>> MaakVerdeling([FromRoute] int id)
    {
        try
        {
            var aanwezigheden = Aservice.GetAanwezighedenOpSpeeldag(id);
            return Ok(service.MaakVerdeling(aanwezigheden, id));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
