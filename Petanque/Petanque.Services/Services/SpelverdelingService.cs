using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Linq.Expressions;

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
            public int Terrein;
            public List<int> TeamLeden, Tegenspelers;
        }
        public IEnumerable<SpelverdelingResponseContract> MaakVerdeling(IEnumerable<AanwezigheidResponseContract> aanwezigheden, int speeldagId)
        {
            _logger.LogCritical("Starting MaakVerdeling");

            const int maxAantalTerreinen = 10;
            const int aantalSpelrondes = 3;

            const int minAantalSpelersPerTeam = 2;
            const int maxAantalSpelersPerTeam = 3;

            int aantalGebruikteTerreinen; // aantal GEBRUIKTE terreinen
            List<int> masterSpelerList; // lijst van Volgnrs van aanwezige spelers
            Dictionary<string, int> aantalSpelersPerTerreinPerTeam; // key="terrein,team", value=aantalSpelers
            Dictionary<string, int> spelverdelingsInfo; // key="spelronde,terrein,team,nrInTeam", value=spelerVolgnr
            Dictionary<string, smartDetails> smartDetailsDictionary; // key="spelronde,spelerVolgnr", value="Terrein,TeamLeden,Tegenspelers"

            // STAP 1: Vul 'masterSpelerList', check aantal aanwezigen en terreinen, vul 'aantalSpelersPerTerreinPerTeam'
            {
                if (aanwezigheden == null)
                    throw new InvalidOperationException($"BUG: Aanwezigheden zijn null. Dit mag niet gebeuren.");

                masterSpelerList = aanwezigheden.Select(a => a.SpelerVolgnr).ToList();
                if (masterSpelerList.Distinct().Count() != masterSpelerList.Count())
                    throw new InvalidOperationException($"BUG: Er zitten dubbele 'SpelerVolgnr's in de lijst 'aanwezigheden'.");

                int aantalAanwezigen = masterSpelerList.Count();
                if (aantalAanwezigen == 0)
                    throw new InvalidOperationException($"Er zijn nog geen aanwezigen aangeduid op deze speeldag");
                if ((int)Math.Ceiling((double)aantalAanwezigen / maxAantalSpelersPerTeam / 2) > maxAantalTerreinen)
                    throw new InvalidOperationException($"Er zijn {maxAantalTerreinen} terreinen beschikbaar. Er is dus slechts plaats voor {maxAantalSpelersPerTeam * 2 * maxAantalTerreinen} van de {aantalAanwezigen} aanwezigen. (Verhoog evt. 'maxAantalSpelersPerTeam')");

                aantalGebruikteTerreinen = (int)Math.Floor((double)aantalAanwezigen / minAantalSpelersPerTeam / 2);
                if (aantalGebruikteTerreinen < 1)
                    throw new InvalidOperationException($"Er zijn slechts {aantalAanwezigen} aanwezigen. Dit is onvoldoende als je minstens {minAantalSpelersPerTeam} spelers per team wilt. (Verlaag evt. 'minAantalSpelersPerTeam')");
                if ((int)Math.Ceiling((double)aantalAanwezigen / aantalGebruikteTerreinen / 2) > maxAantalSpelersPerTeam)
                    throw new InvalidOperationException($"Met {aantalAanwezigen} aanwezigen kan er geen spelverdeling gemaakt worden met minstens {minAantalSpelersPerTeam} en maximaal {maxAantalSpelersPerTeam} spelers per team. (Verlaag evt. 'minAantalSpelersPerTeam' of verhoog 'maxAantalSpelersPerTeam')");

                aantalSpelersPerTerreinPerTeam = new Dictionary<string, int>();
                int totaalAantalSpelers = 0;
                int terrein;
                for (terrein = 1; terrein <= aantalGebruikteTerreinen; terrein++)
                {
                    aantalSpelersPerTerreinPerTeam[$"{terrein},A"] = minAantalSpelersPerTeam;
                    aantalSpelersPerTerreinPerTeam[$"{terrein},B"] = minAantalSpelersPerTeam;
                    totaalAantalSpelers += 2 * minAantalSpelersPerTeam;
                }
                if (totaalAantalSpelers > aantalAanwezigen)
                    throw new InvalidOperationException($"BUG: totaalAantalSpelers={totaalAantalSpelers} > aantalAanwezigen={aantalAanwezigen}");

                terrein = 1; char team = 'A';
                while (totaalAantalSpelers < aantalAanwezigen)
                {
                    aantalSpelersPerTerreinPerTeam[$"{terrein},{team}"]++;
                    totaalAantalSpelers++;
                    team++;
                    if (team == 'C')
                    {
                        team = 'A';
                        terrein++;
                        if (terrein > aantalGebruikteTerreinen) terrein = 1;
                    }
                }
            }
            // STAP 2: Spelverdeling maken (lokaal in Dictionary)
            {
                smartDetailsDictionary = new Dictionary<string, smartDetails>();
                spelverdelingsInfo = new Dictionary<string, int>();
                for (int spelronde = 1; spelronde <= aantalSpelrondes; spelronde++)
                {
                    var beschikbareSpelers = new List<int>(masterSpelerList);
                    var selectieVoorkeurScores = beschikbareSpelers.ToDictionary(n => n, n => 100);
                    for (int terrein = 1; terrein <= aantalGebruikteTerreinen; terrein++)
                    {
                        var teamListDict = new Dictionary<char, List<int>>();
                        teamListDict['A'] = new List<int>();
                        teamListDict['B'] = new List<int>();
                        foreach (char team in new List<char> { 'A', 'B' })
                        {
                            char otherTeam = (team == 'A') ? 'B' : 'A';
                            if (spelronde > 1) { selectieVoorkeurScores = beschikbareSpelers.ToDictionary(n => n, n => 100); }
                            for (int nrInTeam = 1; nrInTeam <= aantalSpelersPerTerreinPerTeam[$"{terrein},{team}"]; nrInTeam++)
                            {
                                // pas 'selectieVoorkeurScores' aan
                                for (int spelronde2 = 1; spelronde2 < spelronde; spelronde2++)
                                {
                                    foreach (int speler in beschikbareSpelers)
                                    {
                                        if (nrInTeam == 1)
                                        {
                                            if (smartDetailsDictionary[$"{spelronde2},{speler}"].Terrein == terrein)
                                                selectieVoorkeurScores[speler] -= spelronde2; // speelde al eens op dit terrein

                                            if (aantalSpelersPerTerreinPerTeam[$"{terrein},{team}"] > minAantalSpelersPerTeam) // zal nu in te groot team zitten
                                            {
                                                if (smartDetailsDictionary[$"{spelronde2},{speler}"].TeamLeden.Count > minAantalSpelersPerTeam)
                                                    selectieVoorkeurScores[speler] -= spelronde2 + 10; // speelde al eens in een te groot team
                                                if (smartDetailsDictionary[$"{spelronde2},{speler}"].Tegenspelers.Count > minAantalSpelersPerTeam)
                                                    selectieVoorkeurScores[speler] -= spelronde2 + 8; // speelde al eens TEGEN een te groot team
                                            }
                                            if (aantalSpelersPerTerreinPerTeam[$"{terrein},{otherTeam}"] > minAantalSpelersPerTeam) // zal TEGEN te groot team spelen
                                            {
                                                if (smartDetailsDictionary[$"{spelronde2},{speler}"].TeamLeden.Count > minAantalSpelersPerTeam)
                                                    selectieVoorkeurScores[speler] -= spelronde2 + 8; // speelde zelf al eens in een te groot team
                                                if (smartDetailsDictionary[$"{spelronde2},{speler}"].Tegenspelers.Count > minAantalSpelersPerTeam)
                                                    selectieVoorkeurScores[speler] -= spelronde2 + 6; // speelde al eens TEGEN een te groot team

                                            }
                                            if (team == 'B')
                                            {
                                                foreach (int speler2 in teamListDict['A'])
                                                {
                                                    if (smartDetailsDictionary[$"{spelronde2},{speler}"].TeamLeden.Contains(speler2))
                                                        selectieVoorkeurScores[speler] -= spelronde2 + 14; // was Teamlid, zou nu Tegenspeler zijn
                                                    if (smartDetailsDictionary[$"{spelronde2},{speler}"].Tegenspelers.Contains(speler2))
                                                        selectieVoorkeurScores[speler] -= spelronde2 + 17; // was Tegenspeler, zou nu opnieuw Tegenspeler zijn
                                                }
                                            }
                                        }
                                        else // nrInTeam > 1
                                        {
                                            int vorigeSpeler = spelverdelingsInfo[$"{spelronde},{terrein},{team},{nrInTeam - 1}"];
                                            if (smartDetailsDictionary[$"{spelronde2},{speler}"].TeamLeden.Contains(vorigeSpeler))
                                                selectieVoorkeurScores[speler] -= spelronde2 + 20; // was TeamLid, zou nu opnieuw TeamLid zijn
                                            if (smartDetailsDictionary[$"{spelronde2},{speler}"].Tegenspelers.Contains(vorigeSpeler))
                                                selectieVoorkeurScores[speler] -= spelronde2 + 14; // was Tegenspeler, zou nu TeamLid zijn
                                        }
                                    }
                                    /*foreach (int speler in beschikbareSpelers)
                                    {
                                        _logger.LogInformation($"speler={speler}, score={selectieVoorkeurScores[speler]}");
                                    }*/
                                }
                                int maxVal = selectieVoorkeurScores.Values.Max();
                                int count = selectieVoorkeurScores.Where(kvp => kvp.Value == maxVal).Count();
                                int s = selectieVoorkeurScores.Where(kvp => kvp.Value == maxVal).ToDictionary().Keys.ElementAt(_random.Next(count));
                                _logger.LogCritical($"spelronde={spelronde}, terrein={terrein}, team={team}, nrInTeam={nrInTeam}, maxVal={maxVal}, count={count}: speler={s}");
                                beschikbareSpelers.Remove(s);
                                selectieVoorkeurScores.Remove(s);
                                spelverdelingsInfo[$"{spelronde},{terrein},{team},{nrInTeam}"] = s;
                                teamListDict[team].Add(s);
                            }
                        }
                        // vul smartDetailsDictionary in, behalve de laatste keer
                        if (spelronde < aantalSpelrondes)
                        {
                            //_logger.LogInformation($"--- vul smartDetailsDictionary in");
                            foreach (char team in new List<char> { 'A', 'B' })
                            {
                                char otherTeam = (team == 'A') ? 'B' : 'A';
                                for (int nrInTeam = 1; nrInTeam <= aantalSpelersPerTerreinPerTeam[$"{terrein},{team}"]; nrInTeam++)
                                {
                                    int s = spelverdelingsInfo[$"{spelronde},{terrein},{team},{nrInTeam}"];
                                    smartDetailsDictionary[$"{spelronde},{s}"] = new smartDetails
                                    {
                                        Terrein = terrein,
                                        TeamLeden = new List<int>(teamListDict[team]),
                                        Tegenspelers = new List<int>(teamListDict[otherTeam])
                                    };
                                }
                            }
                        }
                    }
                }
            }
            // STAP 3: DELETE old Spel + Spelverdeling from DB
            {
                var oudeSpelIds = _context.Spels.Where(sp => sp.SpeeldagId == speeldagId).Select(sp => sp.SpelId).ToList();
                var oudeSpelverdelingen = _context.Spelverdelings.Where(sv => oudeSpelIds.Contains(sv.SpelId ?? 0));
                _context.Spelverdelings.RemoveRange(oudeSpelverdelingen);

                var oudeSpellen = _context.Spels.Where(sp => sp.SpeeldagId == speeldagId);
                _context.Spels.RemoveRange(oudeSpellen);
                _context.SaveChanges(); // Commit delete
            }
            // STAP 4: INSERT content of (Dictionary) spelverdelingsInfo to DB
            {
                var responses = new List<SpelverdelingResponseContract>();
                for (int spelronde = 1; spelronde <= aantalSpelrondes; spelronde++)
                {
                    for (int terrein = 1; terrein <= aantalGebruikteTerreinen; terrein++)
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

                        foreach (char team in new List<char> { 'A', 'B' })
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
