using Petanque.Contracts;
using Petanque.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services {
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

        public DagKlassementResponseContract? GetById(int id) {
            throw new NotImplementedException();
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
