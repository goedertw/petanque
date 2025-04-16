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
