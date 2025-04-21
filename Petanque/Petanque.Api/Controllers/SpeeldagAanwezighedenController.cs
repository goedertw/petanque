using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers;

[ApiController]
[Route("api/speeldagaanwezigheden")]
public class SpeeldagAanwezigheden(IAanwezigheidService service) : Controller
{
    [HttpGet("{id}")]
    public ActionResult<IEnumerable<AanwezigheidResponseContract>> GetAll([FromRoute] int id)
    {
        return Ok(service.GetAanwezighedenOpSpeeldag(id));
    }
}

