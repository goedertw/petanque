using Microsoft.EntityFrameworkCore;
using Petanque.Services.Interfaces;
using QuestPDF.Helpers;
using Petanque.Storage;
using QuestPDF.Fluent;

namespace Petanque.Services.Services;

public class SeizoensKlassementPDFService : ISeizoensKlassementPDFService
{
    private readonly Id312896PetanqueContext _context;

    public SeizoensKlassementPDFService(Id312896PetanqueContext context)
    {
        _context = context;
    }
    public async Task<Stream> GenerateSeizoensKlassementPdfAsync(int id)
    {
         var klassementen = await _context.Seizoensklassements
                .Where(sk => sk.SeizoensId == id)
                .OrderByDescending(sk => sk.Hoofdpunten).ThenByDescending(sk => sk.PlusMinPunten)
                .ToListAsync();

            if (klassementen == null || !klassementen.Any())
            {
                return null;
            }

            var memoryStream = new MemoryStream();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(595, 842); // A4 formaat in pixels
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().Column(col =>
                    {
                        // Titel met padding
                        col.Item().Element(e => e
                            .PaddingBottom(2)
                            .Text($"VL@S Seizoensklassement {id}")
                            .FontSize(14)
                            .Bold()
                            .AlignCenter());

                        // Ruimte tussen titel en tabel
                        col.Item().Element(e => e.PaddingTop(10));

                        // Tabel opbouw
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
                            int prevHoofdpunten = 0;
                            int prevPlusMinPunten = 0;
                            foreach (var item in klassementen)
                            {
                                bool isEvenRow = rang % 2 == 0;
                                string background = isEvenRow ? Colors.Grey.Lighten4 : Colors.White;

                                if ((item.Hoofdpunten == prevHoofdpunten) && (item.PlusMinPunten == prevPlusMinPunten))
                                    table.Cell().Element(e => e.Background(background).PaddingVertical(2)).Text(' ');
                                else
                                    table.Cell().Element(e => e.Background(background).PaddingVertical(2)).Text(rang.ToString());
                                table.Cell().Element(e => e.Background(background).PaddingVertical(2)).Text($"{item.SpelerNaam} {item.SpelerVoornaam}");
                                table.Cell().Element(e => e.Background(background).PaddingVertical(2)).AlignCenter().Text(item.Hoofdpunten.ToString());
                                table.Cell().Element(e => e.Background(background).PaddingVertical(2)).AlignCenter().Text(item.PlusMinPunten.ToString());

                                rang++;
                                prevHoofdpunten = item.Hoofdpunten;
                                prevPlusMinPunten = item.PlusMinPunten;
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
