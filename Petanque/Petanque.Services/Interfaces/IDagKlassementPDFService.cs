using System;

namespace Petanque.Services.Interfaces;

public interface IDagKlassementPDFService
{
    Task<Stream> GenerateDagKlassementPdfAsync(int id);

}
