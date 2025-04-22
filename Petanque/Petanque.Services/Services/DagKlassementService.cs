using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services.Services {
    public class DagKlassementService(Id312896PetanqueContext context) : IDagKlassementService {
        public DagKlassementResponseContract Create(DagKlassementRequestContract request) {
            var entity = new Dagklassement() {
                SpeeldagId = request.SpeeldagId,
                Hoofdpunten = request.Hoofdpunten,
                PlusMinPunten = request.PlusMinPunten,
                SpelerId = request.SpelerId
            };

            context.Dagklassements.Add(entity);
            context.SaveChanges();

            return MapToContract(entity);
        }

        public IEnumerable<DagKlassementResponseContract>? GetById(int id) {
            var dagklassementen = context.Dagklassements
            .Where(d => d.SpeeldagId == id)
            .ToList();

            return dagklassementen
                .Select(MapToContract)
                .Where(contract => contract != null)
                .ToList()!;
        }
        public IEnumerable<DagKlassementResponseContract> CreateDagKlassementen(SpeeldagResponseContract speeldagData, int id)
        {
            var speeldagId = speeldagData.SpeeldagId;

            var spelersInSpeeldag = context.Aanwezigheids
                .Where(x => x.SpeeldagId == speeldagId)
                .ToDictionary(x => x.SpelerVolgnr, x => x.SpelerId);

            var scorePerSpeler = new Dictionary<int, int>();
            var winsPerSpeler = new Dictionary<int, int>();

            foreach (var spel in speeldagData.Spel)
            {
                if (spel?.Spelverdelingen == null || spel.Spelverdelingen.Count == 0)
                    continue;

                var teamA = spel.Spelverdelingen
                    .Where(v => v.Team == "Team A")
                    .Select(v => v.SpelerVolgnr)
                    .ToList();

                var teamB = spel.Spelverdelingen
                    .Where(v => v.Team == "Team B")
                    .Select(v => v.SpelerVolgnr)
                    .ToList();

                if (teamA.Count == 0 || teamB.Count == 0)
                    continue;

                var scoreA = spel.ScoreA;
                var scoreB = spel.ScoreB;
                var scoreVerschil = scoreA - scoreB;

                // Punten toekennen
                foreach (var speler in teamA)
                {
                    if (!scorePerSpeler.ContainsKey(speler)) scorePerSpeler[speler] = 0;
                    scorePerSpeler[speler] += scoreVerschil;

                    if (scoreA > scoreB) // Team A wint
                    {
                        if (!winsPerSpeler.ContainsKey(speler)) winsPerSpeler[speler] = 0;
                        winsPerSpeler[speler]++;
                    }
                }

                foreach (var speler in teamB)
                {
                    if (!scorePerSpeler.ContainsKey(speler)) scorePerSpeler[speler] = 0;
                    scorePerSpeler[speler] -= scoreVerschil;

                    if (scoreB > scoreA) // Team B wint
                    {
                        if (!winsPerSpeler.ContainsKey(speler)) winsPerSpeler[speler] = 0;
                        winsPerSpeler[speler]++;
                    }
                }
            }

            var dagKlassementen = new List<DagKlassementResponseContract>();

            foreach (var (spelerVolgNr, spelerId) in spelersInSpeeldag)
            {
                var plusMin = scorePerSpeler.TryGetValue(spelerVolgNr, out var punten) ? punten : 0;
                var gewonnenSpellen = winsPerSpeler.TryGetValue(spelerVolgNr, out var wins) ? wins : 0;

                dagKlassementen.Add(new DagKlassementResponseContract
                {
                    SpeeldagId = speeldagId,
                    SpelerId = spelerId,
                    Hoofdpunten = 1 + gewonnenSpellen,
                    PlusMinPunten = plusMin
                });
            }

            var entities = dagKlassementen.Select(k => new Dagklassement
            {
                SpeeldagId = k.SpeeldagId,
                SpelerId = k.SpelerId,
                Hoofdpunten = k.Hoofdpunten,
                PlusMinPunten = k.PlusMinPunten
            }).ToList();

            context.AddRange(entities);
            context.SaveChanges();

            return dagKlassementen;
        }

        private static DagKlassementResponseContract MapToContract(Dagklassement entity) {
            return new DagKlassementResponseContract() {
                DagklassementId = entity.DagklassementId,
                SpeeldagId = entity.SpeeldagId,
                SpelerId = entity.SpelerId,
                Hoofdpunten = entity.Hoofdpunten,
                PlusMinPunten = entity.PlusMinPunten,
            };
        }
    }
}
