using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers
{
    [Route("api/aanwezigheden")]
    [ApiController]
    public class AanwezigheidController(IAanwezigheidService service) : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<AanwezigheidResponseContract> Get([FromRoute] int id)
        {
            var aanwezigheid = service.GetById(id);
            if (aanwezigheid is null) return NotFound();
            return Ok(aanwezigheid);
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
        
        [HttpDelete("{id}")]
        public IActionResult DeleteAanwezigheid(int id)
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
        [HttpGet("check-speler-aanwezig")]
        public ActionResult<object> CheckSpelerAanwezig([FromQuery] int id)
        {
            var aanwezigheden = service.GetAanwezighedenOpSpeler(id);

            if (aanwezigheden.Any())
            {
                var speeldagen = aanwezigheden
                    .Select(a => a.SpeeldagDatum)
                    .Distinct()
                    .ToList();

                return Ok(new { aanwezig = true, speeldagen });
            }

            return Ok(new { aanwezig = false, speeldagen = new List<string>() });
        }

    }
}
