using Microsoft.EntityFrameworkCore;
using Petanque.Services.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Petanque.Storage;
using QuestPDF.Helpers;

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
            // Haal alleen de spelers op waarvan het spelerId voorkomt in het dagklassement voor de opgegeven speeldag
            var spelerIdsInDagklassement = await _context.Dagklassements
                .Where(d => d.SpeeldagId == id)
                .Select(d => d.SpelerId)
                .ToListAsync();

            var spelers = await _context.Spelers
                .Where(sp => spelerIdsInDagklassement.Contains(sp.SpelerId))
                .ToListAsync();

            var dagklassements = await _context.Dagklassements
                .Where(d => d.SpeeldagId == id)
                .ToListAsync();

            if (dagklassements == null || !dagklassements.Any())
            {
                return null;
            }

            var spelersMetScores = spelers.Select(speler =>
            {
                var dagKlassement = dagklassements.FirstOrDefault(dk => dk.SpelerId == speler.SpelerId);
                var score = dagKlassement != null ? dagKlassement.PlusMinPunten : 0;

                return new
                {
                    speler.Naam,
                    speler.Voornaam,
                    Score = score,
                    Hoofdpunten = dagKlassement?.Hoofdpunten ?? 0
                };
            }).OrderByDescending(s => s.Hoofdpunten).ToList();

            var memoryStream = new MemoryStream();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(595, 842);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().Column(col =>
                    {
                        // Voeg titel toe met padding aan de onderkant van de titel
                        col.Item().Element(e => e
                            .PaddingBottom(2)  // Padding aan de onderkant van de titel
                            .Text($"VL@S Speeldag {id}")
                            .FontSize(14)
                            .Bold()
                            .AlignCenter());

                        // Voeg wat extra ruimte boven de tabel toe
                        col.Item().Element(e => e.PaddingTop(10));  // Dit geeft de ruimte tussen de titel en de tabel

                        // Tabel toevoegen
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(25);   // Rang
                                columns.RelativeColumn(3);    // Naam
                                columns.ConstantColumn(35);   // Hoofdpunten
                                columns.ConstantColumn(35);   // +/- punten
                            });

                            int rang = 1;
                            foreach (var speler in spelersMetScores)
                            {
                                bool isEvenRow = rang % 2 == 0;
                                string background = isEvenRow ? Colors.Grey.Lighten4 : Colors.White;

                                table.Cell().Element(e => e.Background(background).PaddingVertical(2)).Text(rang.ToString());
                                table.Cell().Element(e => e.Background(background).PaddingVertical(2)).Text(speler.Voornaam + " " + speler.Naam);
                                table.Cell().Element(e => e.Background(background).PaddingVertical(2)).AlignCenter().Text(speler.Hoofdpunten.ToString() ?? "0");
                                table.Cell().Element(e => e.Background(background).PaddingVertical(2)).AlignCenter().Text(speler.Score.ToString());

                                rang++;
                            }
                        });
                    });
                });
            });

            document.GeneratePdf(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

    }
}
