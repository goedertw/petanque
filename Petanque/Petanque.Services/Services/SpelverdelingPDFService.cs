using System;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;

namespace Petanque.Services.Services;

public class SpelverdelingPDFService : ISpelverdelingPDFService
{
    public Stream GenerateSpelverdelingPDF(IEnumerable<SpelverdelingResponseContract> spelverdelingen)
    {
        var stream = new MemoryStream();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30); // Iets ruimere marges
                page.DefaultTextStyle(x => x.FontSize(12)); // Grotere standaard lettergrootte

                page.Content().Column(col =>
                {
                    var spellen = spelverdelingen
                        .GroupBy(x => x.Spel.SpelId)
                        .OrderBy(x => x.Key);

                    var spelnummer = 1;
                    foreach (var spelGroup in spellen)
                    {
                        var spel = spelGroup.First().Spel;
                        var teamA = spelGroup.Where(x => x.Team == "Team A").ToList();
                        var teamB = spelGroup.Where(x => x.Team == "Team B").ToList();

                        col.Item().PaddingBottom(25).Border(1).Padding(20).Column(spelCol =>
                        {
                            // Spel header gecentreerd met grotere tekst
                            spelCol.Item().Row(row =>
                            {
                                row.RelativeItem().AlignCenter().Container()
                                    .Background(Colors.BlueGrey.Darken2)
                                    .Padding(10)
                                    .Text($"SPEL {spelnummer++}")
                                    .FontSize(16)
                                    .Bold()
                                    .FontColor(Colors.White);
                            });

                            spelCol.Item().PaddingTop(10); // Extra ruimte boven teams

                            // Teams
                            spelCol.Item().Row(row =>
                            {
                                // Team A
                                row.RelativeItem().Column(teamCol =>
                                {
                                    teamCol.Item().Text("Team A").FontSize(14).Bold().Underline();
                                    teamCol.Item().PaddingBottom(5);
                                    foreach (var speler in teamA)
                                    {
                                        var naam = $"{speler.Speler.Voornaam} {speler.Speler.Naam}";
                                        teamCol.Item().Text(naam);
                                    }

                                    teamCol.Item().PaddingTop(10).Text("Punten Team A: ");
                                });

                                // Verticale lijn
                                row.ConstantItem(2).Height(120).Background(Colors.Grey.Lighten2);

                                // Team B
                                row.RelativeItem().Column(teamCol =>
                                {
                                    teamCol.Item().Text("Team B").FontSize(14).Bold().Underline();
                                    teamCol.Item().PaddingBottom(5);
                                    foreach (var speler in teamB)
                                    {
                                        var naam = $"{speler.Speler.Voornaam} {speler.Speler.Naam}";
                                        teamCol.Item().Text(naam);
                                    }

                                    teamCol.Item().PaddingTop(10).Text("Punten Team B: ");
                                });
                            });
                        });
                    }
                });
            });
        })
        .GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }

}
