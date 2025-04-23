using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petanque.Services.Interfaces;
using Petanque.Contracts.Responses;

namespace Petanque.Api.Controllers
{
    [Route("api/pdfspelverdelingen")]
    [ApiController]
    public class SpelverdelingPDFController(ISpelverdelingPDFService service, ISpelverdelingService spelservice) : ControllerBase
    {
        [HttpPost("{speeldagid}/{terreinid}")]
        public IActionResult GeneratePdf([FromRoute] int speeldagid, [FromRoute] int terreinid)
        {
            var spelverdelingen = spelservice.GetBySpeeldagAndTerrein(speeldagid, terreinid);
            if (spelverdelingen == null)
            {
                return NotFound($"Spelverdeling voor Speeldag {speeldagid} niet gevonden.");
            }

            var pdfStream = service.GenerateSpelverdelingPDF(spelverdelingen);

            return File(pdfStream, "application/pdf", $"speeldag_{speeldagid}_terrein_{terreinid}.pdf");
        }
    }
}
