using System;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using Microsoft.EntityFrameworkCore;

namespace Petanque.Services.Services
{
    public class SpelverdelingPDFService : ISpelverdelingPDFService
    {
        private readonly Id312896PetanqueContext _context;

        public SpelverdelingPDFService(Id312896PetanqueContext context)
        {
            _context = context;
        }

        public Stream GenerateSpelverdelingPDF(IEnumerable<SpelverdelingResponseContract> spelverdelingen)
        {
            var stream = new MemoryStream();

            var spellen = spelverdelingen
                .GroupBy(sv => sv.SpelId)
                .Select(g =>
                {
                    var eerste = g.First();
                    return new SpelResponseContract
                    {
                        SpelId = (int)eerste.SpelId,
                        SpeeldagId = eerste.Spel.SpeeldagId,
                        Terrein = eerste.Spel.Terrein,
                        ScoreA = eerste.Spel.ScoreA,
                        ScoreB = eerste.Spel.ScoreB,
                        Spelverdelingen = g.ToList()
                    };
                })
                .ToList();

            int speeldagIdd = spelverdelingen.First().Spel.SpeeldagId ?? throw new InvalidOperationException("SpeeldagId cannot be null.");
            var speeldag = _context.Speeldags.FirstOrDefault(s => s.SpeeldagId == speeldagIdd);
            string datumFormatted = speeldag.Datum.ToString("dddd d MMMM yyyy", new System.Globalization.CultureInfo("nl-NL"));

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content().Column(col =>
                    {
                        var spellenPerTerrein = spellen
                            .GroupBy(spel => spel.Terrein)
                            .OrderBy(g => g.Key);

                        int terreinNummer = 1;

                        foreach (var terreinGroup in spellenPerTerrein)
                        {
                            col.Item().PaddingBottom(15).Row(r =>
                            {
                                r.RelativeItem().Text($"TERREIN: {terreinGroup.Key}")
                                    .FontSize(18)
                                    .Bold()
                                    .Underline();
                                r.RelativeItem().AlignRight().Text(datumFormatted);
                            });

                            int spelnummer = 1;

                            foreach (var spel in terreinGroup)
                            {
                                var teamA = spel.Spelverdelingen.Where(x => x.Team == "Team A").ToList();
                                var teamB = spel.Spelverdelingen.Where(x => x.Team == "Team B").ToList();

                                col.Item().PaddingBottom(20).Border(1).Padding(15).Column(spelCol =>
                                {
                                    spelCol.Item().Row(row =>
                                    {
                                        row.RelativeItem().AlignCenter()
                                            .Background(Colors.BlueGrey.Darken2)
                                            .Padding(8)
                                            .Text($"Spel {spelnummer++}")
                                            .FontSize(16)
                                            .Bold()
                                            .FontColor(Colors.White);
                                    });

                                    spelCol.Item().PaddingTop(10);

                                    spelCol.Item().Row(row =>
                                    {
                                        // Team A
                                        row.RelativeItem().Column(teamCol =>
                                        {
                                            teamCol.Item().Text("Team A").FontSize(14).Bold().Underline();
                                            teamCol.Item().PaddingBottom(5);

                                            foreach (var speler in teamA)
                                            {
                                                var naam = speler.Speler != null
                                                    ? $"{speler.SpelerVolgnr}. {speler.Speler.Naam} {speler.Speler.Voornaam}"
                                                    : $"Onbekende speler (volgnr {speler.SpelerVolgnr})";
                                                teamCol.Item().Text(naam);
                                            }


                                            teamCol.Item().Row(scoreRow =>
                                            {
                                                for (int i = 0; i < 13; i++)
                                                {
                                                    scoreRow.ConstantItem(18)
                                                        .Height(18)
                                                        .Border(1)
                                                        .PaddingRight(2);
                                                }
                                            });
                                            teamCol.Item().PaddingTop(1).Text("  1   2    3    4   5    6   7    8    9  10  11 12 13");
                                            teamCol.Item().PaddingTop(5).Text("Punten Team A:");

                                        });

                                        // Verticale lijn tussen teams
                                        row.ConstantItem(2).Height(120).Background(Colors.Grey.Lighten2);

                                        // Team B
                                        row.RelativeItem().Column(teamCol =>
                                        {
                                            teamCol.Item().Text("Team B").FontSize(14).Bold().Underline();
                                            teamCol.Item().PaddingBottom(5);

                                            foreach (var speler in teamB)
                                            {
                                                var naam = speler.Speler != null
                                                    ? $"{speler.SpelerVolgnr}. {speler.Speler.Naam} {speler.Speler.Voornaam}"
                                                    : $"Onbekende speler (volgnr {speler.SpelerVolgnr})";
                                                teamCol.Item().Text(naam);
                                            }


                                            teamCol.Item().Row(scoreRow =>
                                            {
                                                for (int i = 0; i < 13; i++)
                                                {
                                                    scoreRow.ConstantItem(18)
                                                        .Height(18)
                                                        .Border(1)
                                                        .PaddingRight(2);
                                                }
                                            });
                                            teamCol.Item().PaddingTop(1).Text("  1   2    3    4   5    6   7    8    9  10  11 12 13");
                                            teamCol.Item().PaddingTop(5).Text("Punten Team B:");

                                        });
                                    });
                                });
                            }

                            if (terreinGroup.Key != spellenPerTerrein.Last().Key)
                            {
                                col.Item().PageBreak();
                            }

                            terreinNummer++;
                        }
                    });
                });
            })
            .GeneratePdf(stream);

            stream.Position = 0;
            return stream;
        }
    }
}
