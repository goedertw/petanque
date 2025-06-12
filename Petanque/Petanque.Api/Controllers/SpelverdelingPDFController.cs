using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petanque.Services.Interfaces;
using Petanque.Contracts.Responses;

namespace Petanque.Api.Controllers
{
    [Route("api/pdfspelverdelingen")]
    [ApiController]
    public class SpelverdelingPDFController(ISpelverdelingPDFService service, ISpelverdelingService sdService) : ControllerBase
    {
        [HttpPost("{speeldagid}")]
        public IActionResult GeneratePdf([FromRoute] int speeldagid)
        {
            var spelverdelingen = sdService.GetById(speeldagid);
            if (spelverdelingen == null)
            {
                return NotFound($"Spelverdeling voor Speeldag {speeldagid} niet gevonden.");
            }

            var pdfStream = service.GenerateSpelverdelingPDF(spelverdelingen);

            return File(pdfStream, "application/pdf", $"speeldag_{speeldagid}.pdf");
        }
    }
}
