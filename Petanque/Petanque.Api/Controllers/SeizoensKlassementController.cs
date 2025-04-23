using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers;
[Route("api/seizoensklassementen")]
[ApiController]
public class SeizoensKlassementController(ISeizoensKlassementService service) : ControllerBase
{
    
    [HttpGet("{seizoenId}")]
    public ActionResult<IEnumerable<SeizoensKlassementResponseContract>> Get([FromRoute] int seizoenId) {
        var seizoensklassement = service.GetById(seizoenId);
        if (seizoensklassement is null) return NotFound();
        return Ok(seizoensklassement);
    }
    
    [HttpPost("{seizoenId}/genereer")]
    public IActionResult GenerateSeizoensKlassement(int seizoenId)
    {
        // Genereer het seizoensklassement
        var seizoensKlassementen = service.CreateSeizoensKlassementen(seizoenId);

        if (seizoensKlassementen == null || !seizoensKlassementen.Any())
        {
            return NotFound("Geen gegevens gevonden om het klassement te genereren.");
        }

        return Ok(seizoensKlassementen);
    }
}