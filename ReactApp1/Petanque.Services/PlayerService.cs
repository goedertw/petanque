using Petanque.Contracts;
using Petanque.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services
{
    public class PlayerService(Id312896PetanqueContext context) : IPlayerService
    {
        public PlayerResponseContract Create(PlayerRequestContract request)
        {
            var entity = new Speler()
            {
                Voornaam = request.Voornaam,
                Naam = request.Naam,
                Aanwezigheids = (ICollection<Aanwezigheid>)request.Aanwezigheids,
                Dagklassements = (ICollection<Dagklassement>)request.Dagklassements,
                Seizoensklassements = (ICollection<Seizoensklassement>)request.Seizoensklassements
            };

            context.Spelers.Add(entity);
            context.SaveChanges();

            return MapToContract(entity);
        }

        public PlayerResponseContract? GetById(int id)
        {
            var entity = context.Spelers.Find(id);
            return entity is null ? null : MapToContract(entity);
        }

        private static PlayerResponseContract MapToContract(Speler entity)
        {
            return new PlayerResponseContract()
            {
                SpelerId = entity.SpelerId,
                Voornaam = entity.Voornaam,
                Naam = entity.Naam,
                Aanwezigheids = (ICollection<AanwezigheidResponseContract>)entity.Aanwezigheids,
                Dagklassements = (ICollection<DagKlassementResponseContract>)entity.Dagklassements,
                Seizoensklassements = (ICollection<SeizoensKlassementResponseContract>)entity.Seizoensklassements

            };
        }
    }
}
