using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using System.Linq;

namespace Petanque.Services.Services
{
    public class SpelverdelingService : ISpelverdelingService
    {
        private readonly Random _random = new();
        private readonly Id312896PetanqueContext _context;

        public SpelverdelingService(Id312896PetanqueContext context) => _context = context;

        public IEnumerable<SpelverdelingResponseContract> GetById(int speeldagId)
        {
            // Haal alle spellen op van de speeldag
            var spellen = _context.Spels
                .Where(sp => sp.SpeeldagId == speeldagId)
                .ToList();

            if (!spellen.Any())
                return Enumerable.Empty<SpelverdelingResponseContract>();

            // Haal alle spelverdelingen voor deze spellen op
            var spelIds = spellen.Select(s => s.SpelId).ToList();

            var spelverdelingen = _context.Spelverdelings
                .Where(sv => spelIds.Contains(sv.SpelId ?? 0))
                .ToList();

            // Haal aanwezigheid + speler info op
            var aanwezigheden = _context.Aanwezigheids
                .Include(a => a.Speler)
                .Where(a => a.SpeeldagId == speeldagId)
                .ToList();

            return spelverdelingen.Select(sv =>
            {
                var speler = aanwezigheden
                    .FirstOrDefault(a => a.SpelerVolgnr == sv.SpelerVolgnr)
                    ?.Speler;

                var spel = spellen.FirstOrDefault(sp => sp.SpelId == sv.SpelId);

                return MapToReturn(sv, speler, spel);
            }).ToList();
        }

        public IEnumerable<SpelverdelingResponseContract> MaakVerdeling(IEnumerable<AanwezigheidResponseContract> aanwezigheden, int speeldagId)
        {
            Console.WriteLine("Aanwezigheden:");
            foreach (var a in aanwezigheden)
            {
                Console.WriteLine($"SpelerVolgnr: {a.SpelerVolgnr}");
            }

            if (aanwezigheden == null || !aanwezigheden.Any())
                return Enumerable.Empty<SpelverdelingResponseContract>();

            const int spellenPerSpeler = 3;
            const int maxSpellenPerTerrein = 3;
            const int aantalTerreinen = 5;

            var spelverdelingen = new List<Spelverdeling>();
            var spellen = new List<Spel>();
            var spelerTelling = new Dictionary<int, int>();
            var spellenPerTerrein = new Dictionary<string, int>();

            foreach (var aanwezigheid in aanwezigheden)
            {
                if (!spelerTelling.ContainsKey(aanwezigheid.SpelerVolgnr))
                {
                    spelerTelling[aanwezigheid.SpelerVolgnr] = 0;
                }
            }

            Console.WriteLine("Initiale speler telling:");
            foreach (var kvp in spelerTelling)
            {
                Console.WriteLine($"Speler {kvp.Key} heeft {kvp.Value} spellen gespeeld.");
            }

            int spelId = 1;

            while (spelerTelling.Any(kvp => kvp.Value < spellenPerSpeler))
            {
                Console.WriteLine("Start nieuwe ronde...");

                var beschikbareSpelers = spelerTelling
                    .Where(kvp => kvp.Value < spellenPerSpeler)
                    .OrderBy(_ => _random.Next())
                    .ToList();

                Console.WriteLine("Beschikbare spelers voor deze ronde:");
                foreach (var speler in beschikbareSpelers)
                {
                    Console.WriteLine($"Speler {speler.Key} geselecteerd.");
                }

                if (beschikbareSpelers.Count < 4)
                {
                    Console.WriteLine("Niet genoeg spelers om een team te maken. De ronde stopt.");
                    break;
                }

                // Zoek beschikbaar terrein met minder dan 3 spellen
                string gekozenTerrein = null;
                for (int t = 1; t <= aantalTerreinen; t++)
                {
                    var terreinNaam = $"Terrein {t}";
                    if (!spellenPerTerrein.ContainsKey(terreinNaam))
                        spellenPerTerrein[terreinNaam] = 0;

                    if (spellenPerTerrein[terreinNaam] < maxSpellenPerTerrein)
                    {
                        gekozenTerrein = terreinNaam;
                        break;
                    }
                }

                if (gekozenTerrein == null)
                {
                    Console.WriteLine("Alle terreinen zijn vol (max 3 spellen per terrein). Ronde stopt.");
                    break;
                }

                var teamA = beschikbareSpelers.Take(2).ToList();
                var teamB = beschikbareSpelers.Skip(2).Take(2).ToList();

                int positie = 1;
                foreach (var speler in teamA)
                {
                    spelverdelingen.Add(new Spelverdeling
                    {
                        SpelId = spelId,
                        Team = "Team A",
                        SpelerPositie = $"P{positie++}",
                        SpelerVolgnr = speler.Key
                    });
                    spelerTelling[speler.Key]++;
                }

                positie = 1;
                foreach (var speler in teamB)
                {
                    spelverdelingen.Add(new Spelverdeling
                    {
                        SpelId = spelId,
                        Team = "Team B",
                        SpelerPositie = $"P{positie++}",
                        SpelerVolgnr = speler.Key
                    });
                    spelerTelling[speler.Key]++;
                }

                // Spel aanmaken
                var spel = new Spel
                {
                    SpeeldagId = speeldagId,
                    Terrein = gekozenTerrein,
                    ScoreA = 0,
                    ScoreB = 0,
                    SpelerVolgnr = teamA.First().Key // voorbeeld: eerste speler van Team A
                };

                spellen.Add(spel);
                spellenPerTerrein[gekozenTerrein]++;
                spelId++;
            }

            _context.Spels.AddRange(spellen);
            _context.Spelverdelings.AddRange(spelverdelingen);
            _context.SaveChanges();

            var responses = spelverdelingen.Select(MapToContract).ToList();
            return responses;
        }

        private static SpelverdelingResponseContract MapToContract(Spelverdeling entity)
        {
            return new SpelverdelingResponseContract
            {
                SpelverdelingsId = entity.SpelverdelingsId,
                SpelId = entity.SpelId,
                Team = entity.Team,
                SpelerPositie = entity.SpelerPositie,
                SpelerVolgnr = entity.SpelerVolgnr
            };
        }

        private static SpelverdelingResponseContract MapToReturn(Spelverdeling entity, Speler? speler, Spel? spel)
        {
            return new SpelverdelingResponseContract
            {
                SpelverdelingsId = entity.SpelverdelingsId,
                SpelId = entity.SpelId,
                Team = entity.Team,
                SpelerPositie = entity.SpelerPositie,
                SpelerVolgnr = entity.SpelerVolgnr,
                Speler = speler == null ? null : new PlayerResponseContract
                {
                    SpelerId = speler.SpelerId,
                    Voornaam = speler.Voornaam,
                    Naam = speler.Naam
                },
                Spel = spel == null ? null : new SpelResponseContract
                {
                    SpelId = spel.SpelId,
                    SpeeldagId = spel.SpeeldagId,
                    Terrein = spel.Terrein
                }
            };
        }
    }
}
