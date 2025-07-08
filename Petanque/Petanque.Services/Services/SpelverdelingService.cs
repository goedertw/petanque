using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace Petanque.Services.Services
{
    public class SpelverdelingService : ISpelverdelingService
    {
        private readonly Random _random = new();
        private readonly Id312896PetanqueContext _context;
        private readonly ILogger _logger;

        public SpelverdelingService(Id312896PetanqueContext context, ILogger<SpelverdelingService> logger)
        {
            _context = context;
            _logger = logger;
        }

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

        public struct smartDetails
        {
            public int Terrein, TotaalAantalSpelers;
            public List<int> TeamLeden, Tegenspelers;
        }
        public IEnumerable<SpelverdelingResponseContract> MaakVerdeling(IEnumerable<AanwezigheidResponseContract> aanwezigheden, int speeldagId)
        {
            _logger.LogCritical("Starting MaakVerdeling");

            const int maxAantalTerreinen = 10;
            const int aantalSpelrondes = 1;

            const int minAantalSpelersPerTeam = 2;
            const int maxAantalSpelersPerTeam = 3;

            int aantalAanwezigen;
            int aantalTerreinen;

            if (aanwezigheden == null)
                throw new InvalidOperationException($"BUG: Aanwezigheden zijn null. Dit mag niet gebeuren.");

            aantalAanwezigen = aanwezigheden.Count();
            if (aantalAanwezigen == 0)
                throw new InvalidOperationException($"Er zijn nog geen aanwezigen aangeduid op deze speeldag");

            if ((int)Math.Ceiling((double)aantalAanwezigen / maxAantalSpelersPerTeam / 2) > maxAantalTerreinen)
                throw new InvalidOperationException($"Er zijn {maxAantalTerreinen} terreinen beschikbaar. Er is dus slechts plaats voor {maxAantalSpelersPerTeam * 2 * maxAantalTerreinen} van de {aantalAanwezigen} aanwezigen.");

            aantalTerreinen = (int)Math.Floor((double)aantalAanwezigen / minAantalSpelersPerTeam / 2);
            if (aantalTerreinen < 1)
                throw new InvalidOperationException($"Er zijn slechts {aantalAanwezigen} aanwezigen. Dit is onvoldoende als je minstens {minAantalSpelersPerTeam} spelers per team wilt.");

            // STAP 1: Verwijder oude Spel + Spelverdeling voor deze speeldag
            var oudeSpelIds = _context.Spels
                .Where(sp => sp.SpeeldagId == speeldagId)
                .Select(sp => sp.SpelId)
                .ToList();

            var oudeSpelverdelingen = _context.Spelverdelings
                .Where(sv => oudeSpelIds.Contains(sv.SpelId ?? 0));

            _context.Spelverdelings.RemoveRange(oudeSpelverdelingen);

            var oudeSpellen = _context.Spels
                .Where(sp => sp.SpeeldagId == speeldagId);

            _context.Spels.RemoveRange(oudeSpellen);

            _context.SaveChanges(); // Commit delete

            // STAP 2: Vul de lijst 'aantalSpelersPerTerreinPerTeam', sommige terreinen/teams hebben extra spelers nodig
            var aantalSpelersPerTerreinPerTeam = new Dictionary<string, int>(); // key="terrein,team"
            int totaalAantalSpelers = 0;
            int terrein;
            char team;
            for (terrein = 1; terrein <= aantalTerreinen; terrein++)
            {
                aantalSpelersPerTerreinPerTeam[$"{terrein},A"] = minAantalSpelersPerTeam;
                aantalSpelersPerTerreinPerTeam[$"{terrein},B"] = minAantalSpelersPerTeam;
                totaalAantalSpelers += 2 * minAantalSpelersPerTeam;
            }
            if (totaalAantalSpelers > aantalAanwezigen)
                throw new InvalidOperationException($"BUG: totaalAantalSpelers={totaalAantalSpelers} > aantalAanwezigen={aantalAanwezigen}");

            terrein = 1; team = 'A';
            while (totaalAantalSpelers < aantalAanwezigen)
            {
                aantalSpelersPerTerreinPerTeam[$"{terrein},{team}"]++;
                totaalAantalSpelers++;
                team++;
                if (team == 'C')
                {
                    team = 'A';
                    terrein++;
                    if (terrein > aantalTerreinen) terrein = 1;
                }
            }

            // STAP 3: Definieer de nodige lijsten met spelverdelingsinfo
            // var aantalSpelersPerTerreinPerTeam = new Dictionary<string, int>(); // key="terrein,team"
            var spelverdelingsInfo = new Dictionary<string,int>(); // key="spelronde,terrein,team,nrInTeam", value=spelerVolgnr
            var smartDetailsDictionary = new Dictionary<string, smartDetails>(); // key="spelronde,spelerVolgnr"
            var masterSpelerList = aanwezigheden
                    .GroupBy(a => a.SpelerVolgnr)
                    .Select(g => g.First().SpelerVolgnr)
                    .ToList();

            // STAP 4: Spelverdeling voor eerste ronde (gewoon random)
            int spelronde = 1;
            var spelerList = new List<int>(masterSpelerList);

            for (terrein = 1; terrein <= aantalTerreinen; terrein++)
            {
                var teamListDict = new Dictionary<char, List<int>>();
                for (team = 'A'; team <= 'B'; team++)
                {
                    teamListDict[team] = new List<int>();
                    for (int nrInTeam = 1; nrInTeam <= aantalSpelersPerTerreinPerTeam[$"{terrein},{team}"]; nrInTeam++)
                    {
                        int i = _random.Next(spelerList.Count());
                        _logger.LogCritical($"index i={i}");
                        int s = spelerList[i];
                        spelerList.Remove(s);
                        spelverdelingsInfo[$"{spelronde},{terrein},{team},{nrInTeam}"] = s;
                        teamListDict[team].Add(s);
                    }
                }
                // vul smartDetailsDictionary in
                for (team = 'A'; team <= 'B'; team++)
                {
                    for (int nrInTeam = 1; nrInTeam <= aantalSpelersPerTerreinPerTeam[$"{terrein},{team}"]; nrInTeam++)
                    {
                        int s = spelverdelingsInfo[$"{spelronde},{terrein},{team},{nrInTeam}"];
                        if (team == 'A')
                        {
                            smartDetailsDictionary[$"{spelronde},{s}"] = new smartDetails
                            {
                                Terrein = terrein,
                                TotaalAantalSpelers = teamListDict['A'].Count() + teamListDict['B'].Count(),
                                TeamLeden = new List<int>(teamListDict['A']),
                                Tegenspelers = new List<int>(teamListDict['B'])
                            };
                        }
                        else if(team == 'B')
                        {
                            smartDetailsDictionary[$"{spelronde},{s}"] = new smartDetails
                            {
                                Terrein = terrein,
                                TotaalAantalSpelers = teamListDict['A'].Count() + teamListDict['B'].Count(),
                                TeamLeden = new List<int>(teamListDict['B']),
                                Tegenspelers = new List<int>(teamListDict['A'])
                            };
                        }
                    }
                }
            }

            // STAP 5: INSERT in DB
            var responses = new List<SpelverdelingResponseContract>();
            for (spelronde = 1; spelronde <= aantalSpelrondes; spelronde++)
            {
                for (terrein = 1; terrein <= aantalTerreinen; terrein++)
                {
                    var spel = new Spel
                    {
                        SpeeldagId = speeldagId,
                        Terrein = $"Terrein {terrein}",
                        ScoreA = 0,
                        ScoreB = 0,
                        SpelerVolgnr = spelverdelingsInfo[$"{spelronde},{terrein},A,1"]
                    };
                    _context.Spels.Add(spel);
                    _context.SaveChanges();

                    for (team = 'A'; team <= 'B'; team++)
                    {
                        for (int nrInTeam = 1; nrInTeam <= aantalSpelersPerTerreinPerTeam[$"{terrein},{team}"]; nrInTeam++)
                        {
                            _context.Spelverdelings.Add(new Spelverdeling
                            {
                                SpelId = spel.SpelId,
                                Team = $"Team {team}",
                                SpelerPositie = $"P{nrInTeam}",
                                SpelerVolgnr = spelverdelingsInfo[$"{spelronde},{terrein},{team},{nrInTeam}"],
                                SpelerId = spelverdelingsInfo[$"{spelronde},{terrein},{team},{nrInTeam}"]
                            });
                        }
                    }
                    _context.SaveChanges();
                    responses.AddRange(_context.Spelverdelings
                        .Where(v => v.SpelId == spel.SpelId)
                        .Select(MapToContract)
                        .ToList());
                }
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
            if (speler == null) throw new ArgumentNullException(nameof(speler), "Speler mag niet null zijn.");
            if (spel == null) throw new ArgumentNullException(nameof(spel), "Spel mag niet null zijn.");

            return new SpelverdelingResponseContract
            {
                SpelverdelingsId = entity.SpelverdelingsId,
                SpelId = entity.SpelId,
                Team = entity.Team,
                SpelerPositie = entity.SpelerPositie,
                SpelerVolgnr = entity.SpelerVolgnr,
                Speler = new PlayerResponseContract
                {
                    SpelerId = speler.SpelerId,
                    Voornaam = speler.Voornaam,
                    Naam = speler.Naam
                },
                Spel = new SpelResponseContract
                {
                    SpelId = spel.SpelId,
                    SpeeldagId = spel.SpeeldagId,
                    Terrein = spel.Terrein
                }
            };
        }
        public IEnumerable<SpelverdelingResponseContract> GetBySpeeldagAndTerrein(int speeldag, int terrein)
        {
            var spellen = _context.Spels
                .Where(sp => sp.SpeeldagId == speeldag && sp.Terrein == $"Terrein {terrein}")
                .ToList();

            if (!spellen.Any())
                return Enumerable.Empty<SpelverdelingResponseContract>();

            var spelIds = spellen.Select(s => s.SpelId).ToList();

            var spelverdelingen = _context.Spelverdelings
                .Where(sv => spelIds.Contains(sv.SpelId ?? 0))
                .ToList();

            var aanwezigheden = _context.Aanwezigheids
                .Include(a => a.Speler)
                .Where(a => a.SpeeldagId == speeldag)
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
    }
}
