namespace Petanque.Services.Interfaces;

public interface ISeizoensKlassementPDFService
{
    Task<Stream> GenerateSeizoensKlassementPdfAsync(int id);
}