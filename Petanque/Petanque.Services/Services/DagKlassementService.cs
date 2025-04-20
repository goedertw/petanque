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
        public IEnumerable<DagKlassementResponseContract> CreateDagKlassementen(SpeeldagResponseContract spelerscores, int id)
        {
            var speeldagId = spelerscores.SpeeldagId;

            // Mapping SpelerVolgnr -> SpelerId
            var spelersInSpeeldag = context.Aanwezigheids
                .Where(x => x.SpeeldagId == speeldagId)
                .ToDictionary(x => x.SpelerVolgnr, x => x.SpelerId);

            // Dictionary om cumulatieve scores op te slaan per SpelerVolgNr
            var spelerScoreTeller = new Dictionary<int, int>();

            foreach (var spel in spelerscores.Spellen)
            {
                foreach (var score in spel.SpelerScores)
                {
                    if (!spelerScoreTeller.ContainsKey(score.SpelerVolgNr))
                        spelerScoreTeller[score.SpelerVolgNr] = 0;

                    spelerScoreTeller[score.SpelerVolgNr] += score.ScoreA - score.ScoreB;
                }
            }

            var klassementen = new List<DagKlassementResponseContract>();

            foreach (var kvp in spelerScoreTeller)
            {
                var spelerVolgnr = kvp.Key;
                var plusMin = kvp.Value;

                if (spelersInSpeeldag.TryGetValue(spelerVolgnr, out var spelerId))
                {
                    klassementen.Add(new DagKlassementResponseContract
                    {
                        SpeeldagId = speeldagId,
                        SpelerId = spelerId,
                        Hoofdpunten = 1, // altijd 1
                        PlusMinPunten = plusMin
                    });
                }
            }

            var klassementEntities = klassementen.Select(k => new Dagklassement
            {
                SpeeldagId = k.SpeeldagId,
                SpelerId = k.SpelerId,
                Hoofdpunten = k.Hoofdpunten,
                PlusMinPunten = k.PlusMinPunten
            }).ToList();

            context.AddRange(klassementEntities);
            context.SaveChanges();

            return klassementen;
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
