using Microsoft.AspNetCore.Mvc;
using Petanque.Services.Interfaces;
using System.Threading.Tasks;

namespace Petanque.Api.Controllers
{
    [Route("api/pdfdagklassementen")]
    [ApiController]
    public class DagKlassementPDFController : ControllerBase
    {
        private readonly IDagKlassementPDFService _dagKlassementPDFService;

        // Via dependency injection de service injecteren
        public DagKlassementPDFController(IDagKlassementPDFService dagKlassementPDFService)
        {
            _dagKlassementPDFService = dagKlassementPDFService;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> GeneratePdf(int id)
        {
            // Roep de service aan om de PDF te genereren
            var pdfStream = await _dagKlassementPDFService.GenerateDagKlassementPdfAsync(id);

            if (pdfStream == null)
            {
                return NotFound($"Dagklassement voor id {id} niet gevonden.");
            }

            // Geef de PDF terug als een bestand naar de client
            return File(pdfStream, "application/pdf", $"DagKlassement_{id}.pdf");
        }
    }
}
