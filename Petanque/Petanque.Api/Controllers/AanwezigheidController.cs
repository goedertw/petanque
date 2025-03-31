using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts;
using Petanque.Services;

namespace Petanque.Api.Controllers
{
    [Route("api/aanwezigheden")]
    [ApiController]
    public class AanwezigheidController(IAanwezigheidService service) : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<AanwezigheidResponseContract> Get([FromRoute] int id)
        {
            var dagklassement = service.GetById(id);
            if (dagklassement is null) return NotFound();
            return Ok(dagklassement);
        }

        [HttpPost]
        public ActionResult<AanwezigheidResponseContract> Create(
            [FromBody] AanwezigheidRequestContract request)
        {
            var created = service.Create(request);
            return CreatedAtAction(nameof(Get), new { id = created.AanwezigheidId }, created);
        }
        [HttpGet]
        public ActionResult<IEnumerable<AanwezigheidResponseContract>> GetAll()
        {
            return Ok(service.GetAll());
        }
    }
}
