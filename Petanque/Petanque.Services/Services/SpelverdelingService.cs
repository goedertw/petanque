using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using System.Collections.Generic;
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

            const int aantalTerreinen = 5;
            const int spellenPerSpeler = 3;

            var spelerSpellenTelling = aanwezigheden.ToDictionary(a => a.SpelerVolgnr, _ => 0);
            var spelerSpelPerSpelId = new Dictionary<int, HashSet<int>>();
            var gebruikteSpelersPerSpelId = new Dictionary<int, HashSet<int>>();
            var gebruikteCombinaties = new HashSet<HashSet<int>>(); // Houdt combinaties van spelers bij
            var responses = new List<SpelverdelingResponseContract>();
            int spelNr = 1;

            while (spelerSpellenTelling.Any(kvp => kvp.Value < spellenPerSpeler))
            {
                for (int terreinNr = 1; terreinNr <= aantalTerreinen; terreinNr++)
                {
                    string terreinNaam = $"Terrein {terreinNr}";

                    var gebruikteSpelers = gebruikteSpelersPerSpelId.ContainsKey(spelNr)
                        ? gebruikteSpelersPerSpelId[spelNr]
                        : new HashSet<int>();

                    var eligiblePlayers = spelerSpellenTelling
                        .Where(kvp => kvp.Value < spellenPerSpeler && !gebruikteSpelers.Contains(kvp.Key))
                        .Select(kvp => kvp.Key)
                        .OrderBy(_ => _random.Next())
                        .ToList();

                    if (eligiblePlayers.Count < 2)
                        continue;

                    List<int> teamAPlayers = new();
                    List<int> teamBPlayers = new();

                    if (eligiblePlayers.Count >= 4)
                    {
                        // Probeer zoveel mogelijk nieuwe combinaties te maken
                        var combinationA = new HashSet<int> { eligiblePlayers[0], eligiblePlayers[1] };
                        var combinationB = new HashSet<int> { eligiblePlayers[2], eligiblePlayers[3] };

                        if (gebruikteCombinaties.Contains(combinationA) || gebruikteCombinaties.Contains(combinationB))
                        {
                            eligiblePlayers = eligiblePlayers.OrderBy(_ => _random.Next()).ToList(); // Shuffle if previous combination found
                            combinationA = new HashSet<int> { eligiblePlayers[0], eligiblePlayers[1] };
                            combinationB = new HashSet<int> { eligiblePlayers[2], eligiblePlayers[3] };
                        }

                        teamAPlayers.Add(eligiblePlayers[0]);
                        teamAPlayers.Add(eligiblePlayers[1]);
                        teamBPlayers.Add(eligiblePlayers[2]);
                        teamBPlayers.Add(eligiblePlayers[3]);

                        // Voeg de nieuwe combinaties toe aan de set
                        gebruikteCombinaties.Add(combinationA);
                        gebruikteCombinaties.Add(combinationB);
                    }
                    else if (eligiblePlayers.Count == 3)
                    {
                        teamAPlayers.Add(eligiblePlayers[0]);
                        teamBPlayers.Add(eligiblePlayers[1]);
                        teamBPlayers.Add(eligiblePlayers[2]);
                    }
                    else // eligiblePlayers.Count == 2
                    {
                        teamAPlayers.Add(eligiblePlayers[0]);
                        teamBPlayers.Add(eligiblePlayers[1]);
                    }

                    var spel = new Spel
                    {
                        SpeeldagId = speeldagId,
                        Terrein = terreinNaam,
                        ScoreA = 0,
                        ScoreB = 0,
                        SpelerVolgnr = teamAPlayers.First()
                    };
                    _context.Spels.Add(spel);
                    _context.SaveChanges();

                    if (!spelerSpelPerSpelId.ContainsKey(spelNr))
                        spelerSpelPerSpelId[spelNr] = new HashSet<int>();

                    int positie = 1;
                    foreach (var spelerId in teamAPlayers)
                    {
                        _context.Spelverdelings.Add(new Spelverdeling
                        {
                            SpelId = spel.SpelId,
                            Team = "Team A",
                            SpelerPositie = $"P{positie++}",
                            SpelerVolgnr = spelerId
                        });
                        spelerSpellenTelling[spelerId]++;
                        spelerSpelPerSpelId[spelNr].Add(spelerId);
                    }

                    positie = 1;
                    foreach (var spelerId in teamBPlayers)
                    {
                        _context.Spelverdelings.Add(new Spelverdeling
                        {
                            SpelId = spel.SpelId,
                            Team = "Team B",
                            SpelerPositie = $"P{positie++}",
                            SpelerVolgnr = spelerId
                        });
                        spelerSpellenTelling[spelerId]++;
                        spelerSpelPerSpelId[spelNr].Add(spelerId);
                    }

                    // Voeg de gebruikte spelers voor dit spel toe aan de lijst
                    if (!gebruikteSpelersPerSpelId.ContainsKey(spelNr))
                        gebruikteSpelersPerSpelId[spelNr] = new HashSet<int>();

                    foreach (var spelerId in teamAPlayers.Concat(teamBPlayers))
                    {
                        gebruikteSpelersPerSpelId[spelNr].Add(spelerId);
                    }

                    _context.SaveChanges();

                    responses.AddRange(_context.Spelverdelings
                        .Where(v => v.SpelId == spel.SpelId)
                        .Select(MapToContract)
                        .ToList());
                }

                spelNr++;
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
