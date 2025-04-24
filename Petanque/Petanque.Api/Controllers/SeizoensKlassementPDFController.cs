using Microsoft.AspNetCore.Mvc;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers;

[Route("api/pdfseizoensklassementen")]
[ApiController]

public class SeizoensKlassementPDFController : ControllerBase
{
    private readonly ISeizoensKlassementPDFService _seizoensKlassementPDFService;

    public SeizoensKlassementPDFController(ISeizoensKlassementPDFService service)
    {
        _seizoensKlassementPDFService = service;
    }
    
    [HttpPost("{id}")]
    public async Task<IActionResult> GeneratePdf(int id)
    {
        // Roep de service aan om de PDF te genereren
        var pdfStream = await _seizoensKlassementPDFService.GenerateSeizoensKlassementPdfAsync(id);

        if (pdfStream == null)
        {
            return NotFound($"Seizoensklassement voor id {id} niet gevonden.");
        }

        // Geef de PDF terug als een bestand naar de client
        return File(pdfStream, "application/pdf", $"Seizoensklassement{id}.pdf");
    }
}