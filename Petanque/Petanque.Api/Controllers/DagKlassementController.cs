using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts;
using Petanque.Services;

namespace Petanque.Api.Controllers {
    [Route("api/dagklassementen")]
    [ApiController]
    public class DagKlassementController(IDagKlassementService service) : ControllerBase {
        [HttpGet("{id}")]
        public ActionResult<DagKlassementResponseContract> Get([FromRoute] int id) {
            var dagklassement = service.GetById(id);
            if (dagklassement is null) return NotFound();
            return Ok(dagklassement);
        }

        [HttpPost]
        public ActionResult<DagKlassementResponseContract> Create(
            [FromBody] DagKlassementRequestContract request) {
            var created = service.Create(request);
            return CreatedAtAction(nameof(Get), new { id = created.SpelerId }, created);
        }
    }
}
