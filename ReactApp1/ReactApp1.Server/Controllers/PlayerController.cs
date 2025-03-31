using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts;
using Petanque.Services;


[ApiController]
[Route("api/players")]
public class PlayerController(IPlayerService service) : ControllerBase
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
        [FromBody] PlayerRequestContract customerRequestContract)
    {
        var created = service.Create(customerRequestContract);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
}