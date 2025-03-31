using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts;
using Petanque.Services;

namespace Petanque.Api.Controllers;
[ApiController]
[Route("api/spelverdeling")]
public class SpelverdelingController(ISpelverdelingService service) : Controller
{
    [HttpGet("{id}")]
    public ActionResult<PlayerResponseContract> Get([FromRoute] int id)
    {
        var spelverdeling = service.GetById(id);
        if (spelverdeling is null) return NotFound();
        return Ok(spelverdeling);
    }
}