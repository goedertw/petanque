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
            var spellen = _context.Spels
                .Where(sp => sp.SpeeldagId == speeldagId)
                .ToList();

            if (!spellen.Any())
                return Enumerable.Empty<SpelverdelingResponseContract>();

            var spelIds = spellen.Select(s => s.SpelId).ToList();

            var spelverdelingen = _context.Spelverdelings
                .Where(sv => spelIds.Contains(sv.SpelId ?? 0))
                .ToList();

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
            if (aanwezigheden == null || !aanwezigheden.Any())
                return Enumerable.Empty<SpelverdelingResponseContract>();

            const int spellenPerSpeler = 3;
            const int maxSpellenPerTerrein = 3;
            const int aantalTerreinen = 5;

            var spelerTelling = new Dictionary<int, int>();
            var spellenPerTerrein = new Dictionary<string, int>();
            var responses = new List<SpelverdelingResponseContract>();

            foreach (var aanwezigheid in aanwezigheden)
            {
                if (!spelerTelling.ContainsKey(aanwezigheid.SpelerVolgnr))
                {
                    spelerTelling[aanwezigheid.SpelerVolgnr] = 0;
                }
            }

            while (spelerTelling.Any(kvp => kvp.Value < spellenPerSpeler))
            {
                var beschikbareSpelers = spelerTelling
                    .Where(kvp => kvp.Value < spellenPerSpeler)
                    .OrderBy(_ => _random.Next())
                    .ToList();

                if (beschikbareSpelers.Count < 4)
                    break;

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
                    break;

                var teamA = beschikbareSpelers.Take(2).ToList();
                var teamB = beschikbareSpelers.Skip(2).Take(2).ToList();

                var spel = new Spel
                {
                    SpeeldagId = speeldagId,
                    Terrein = gekozenTerrein,
                    ScoreA = 0,
                    ScoreB = 0,
                    SpelerVolgnr = teamA.First().Key
                };

                _context.Spels.Add(spel);
                _context.SaveChanges(); // nodig om SpelId te verkrijgen

                spellenPerTerrein[gekozenTerrein]++;

                int positie = 1;
                foreach (var speler in teamA)
                {
                    var verdeling = new Spelverdeling
                    {
                        SpelId = spel.SpelId,
                        Team = "Team A",
                        SpelerPositie = $"P{positie++}",
                        SpelerVolgnr = speler.Key
                    };
                    _context.Spelverdelings.Add(verdeling);
                    spelerTelling[speler.Key]++;
                }

                positie = 1;
                foreach (var speler in teamB)
                {
                    var verdeling = new Spelverdeling
                    {
                        SpelId = spel.SpelId,
                        Team = "Team B",
                        SpelerPositie = $"P{positie++}",
                        SpelerVolgnr = speler.Key
                    };
                    _context.Spelverdelings.Add(verdeling);
                    spelerTelling[speler.Key]++;
                }

                _context.SaveChanges();

                // fetch verdelingen voor dit spel opnieuw om mee te geven in de response
                var verdelingen = _context.Spelverdelings
                    .Where(v => v.SpelId == spel.SpelId)
                    .ToList();

                responses.AddRange(verdelingen.Select(MapToContract));
            }

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
