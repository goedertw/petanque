using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers;
[ApiController]
[Route("api/spelverdelingen")]
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