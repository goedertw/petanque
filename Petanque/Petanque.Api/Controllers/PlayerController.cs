using Microsoft.AspNetCore.Mvc;

namespace Petanque.Api.Controllers;

[ApiController]
[Route("api/players")]
public class PlayerController(IPlayerService service) : Controller
{
        [HttpGet("{id}")]
        public ActionResult<PlayerResponseContract> Get([FromRoute] int id)
        {
            var customer = service.GetById(id);
            if (customer is null) return NotFound();
            return Ok();
        }

        [HttpPost]
        public ActionResult<PlayerResponseContract> Create(
            [FromBody] PlayerRequestContract request)
        {
            var created = service.Create(request);
            return CreatedAtAction(nameof(Get), new { id = created.SpelerId }, created);
        }
}