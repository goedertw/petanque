using Microsoft.EntityFrameworkCore;
using Petanque.Services.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using Petanque.Storage;

namespace Petanque.Services
{
    public class DagKlassementPDFService : IDagKlassementPDFService
    {
        private readonly Id312896PetanqueContext _context;

        // Constructor voor Dependency Injection van DbContext
        public DagKlassementPDFService(Id312896PetanqueContext context)
        {
            _context = context;
        }

        public async Task<Stream> GenerateDagKlassementPdfAsync(int id)
        {
            // Haal alle spelers op
            var spelers = await _context.Spelers.ToListAsync();

            // Haal alle dagklassements op voor de opgegeven SpeeldagId
            var dagklassements = await _context.Dagklassements
                .Where(d => d.SpeeldagId == id)
                .ToListAsync();

            if (dagklassements == null || !dagklassements.Any())
            {
                return null; // Geen dagklassementen gevonden voor het opgegeven id
            }

            // Voeg de scores toe aan de spelers
            var spelersMetScores = spelers.Select(speler =>
            {
                // Zoek het dagklassement voor deze speler
                var dagKlassement = dagklassements.FirstOrDefault(dk => dk.SpelerId == speler.SpelerId);

                // Score van de speler (0 als de speler geen dagklassement heeft)
                var score = dagKlassement != null ? dagKlassement.PlusMinPunten : 0;

                return new
                {
                    speler.Naam,
                    score
                };
            }).ToList();

            // Sorteer de lijst van spelers op score, van hoog naar laag
            var gesorteerdeSpelers = spelersMetScores.OrderByDescending(s => s.score).ToList();

            // Maak een MemoryStream om de gegenereerde PDF in te bewaren
            var memoryStream = new MemoryStream();

            // Genereer de PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(210, 297); // A4 formaat
                    page.Margin(20);

                    page.Content().Column(col =>
                    {
                        col.Item().Text("Dagklassementen");

                        // Voeg de tabel met spelers en scores toe
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // Naam van de speler
                                columns.RelativeColumn(); // Score of rang van de speler
                            });

                            // Voeg de gesorteerde spelers toe aan de tabel
                            foreach (var speler in gesorteerdeSpelers)
                            {
                                int nummer = 1;
                                table.Cell().Text(nummer + " " + speler.Naam);  // Naam van de speler
                                table.Cell().Text(speler.score.ToString());  // Score van de speler
                                nummer++;
                            }
                        });
                    });
                });
            });

            // Genereer de PDF naar de stream
            document.GeneratePdf(memoryStream);

            // Zet de positie van de stream naar het begin voordat we deze teruggeven
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
    }
}
