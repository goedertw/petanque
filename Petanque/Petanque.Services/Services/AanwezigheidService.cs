using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Petanque.Services.Services
{
    public class AanwezigheidService(Id312896PetanqueContext context) : IAanwezigheidService
    {
        public AanwezigheidResponseContract Create(AanwezigheidRequestContract request)
        {
            var entity = new Aanwezigheid()
            {
                SpeeldagId = request.SpeeldagId,
                SpelerId = request.SpelerId,
                SpelerVolgnr = request.SpelerVolgnr
            };

            context.Aanwezigheids.Add(entity);
            context.SaveChanges();

            return MapToContract(entity);
        }
        public AanwezigheidResponseContract? GetById(int id)
        {
            var entity = context.Aanwezigheids.Find(id);
            return entity is null ? null : MapToContract(entity);
        }
        public IEnumerable<AanwezigheidResponseContract> GetAll()
        {
            return context.Aanwezigheids.Select(a => MapToContract(a)).ToList();
        }
        private static AanwezigheidResponseContract MapToContract(Aanwezigheid entity)
        {
            return new AanwezigheidResponseContract()
            {
                AanwezigheidId = entity.AanwezigheidId,
                SpeeldagId = entity.SpeeldagId,
                SpelerId = entity.SpelerId,
                SpelerVolgnr = entity.SpelerVolgnr,
                SpeeldagDatum = entity.Speeldag?.Datum.ToString("yyyy-MM-dd") ?? ""
            };
        }

        public IEnumerable<AanwezigheidResponseContract> GetAanwezighedenOpSpeeldag(int id)
        {
            return context.Aanwezigheids.Select(a => MapToContract(a)).ToList().Where(s => s.SpeeldagId == id);
        }

        public void Delete(int id)
        {
            var entity = context.Aanwezigheids.Find(id);
            if (entity == null)
            {
                throw new ArgumentException($"Aanwezigheid met ID {id} werd niet gevonden.");
            }

            context.Aanwezigheids.Remove(entity);
            context.SaveChanges();
        }

        public IEnumerable<AanwezigheidResponseContract> GetAanwezighedenOpSpeler(int spelerId)
        {
            return context.Aanwezigheids
                .Include(a => a.Speeldag)
                .Where(a => a.SpelerId == spelerId)
                .Select(a => MapToContract(a))
                .ToList();
        }

    }
}
