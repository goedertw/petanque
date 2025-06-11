using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers;

[ApiController]
[Route("api/players")]
public class PlayerController(IPlayerService service) : Controller
{
        [HttpGet("{id}")]
        public ActionResult<PlayerResponseContract> Get([FromRoute] int id)
        {
            var player = service.GetById(id);
            if (player is null) return NotFound();
            return Ok(player);
        }

        [HttpPost]
        public ActionResult<PlayerResponseContract> Create(
            [FromBody] PlayerRequestContract request)
        {
            var created = service.Create(request);
            return CreatedAtAction(nameof(Get), new { id = created.SpelerId }, created);
        }
        [HttpGet]
        public ActionResult<IEnumerable<PlayerResponseContract>> GetAll()
        {
            return Ok(service.GetAll());
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                service.Delete(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
}