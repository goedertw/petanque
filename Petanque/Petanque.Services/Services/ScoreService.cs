using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services.Services
{
    public class ScoreService(Id312896PetanqueContext context) : IScoreService
    {
        public SpelResponseContract Create(SpelRequestContract request)
        {
            //Check op ongeldige invoer
            if (request.ScoreA > 13 || request.ScoreA < 0 || request.ScoreB > 13 || request.ScoreB < 0)
            {
                throw new Exception("De score van een team moet tussen 0 en 13 liggen.");
            }
            var entity = new Spel()
            {
                SpeeldagId = request.SpeeldagId,
                Terrein = request.Terrein,
                SpelerVolgnr = request.SpelerVolgnr,
                ScoreA = request.ScoreA,
                ScoreB = request.ScoreB
            };

            context.Spels.Add(entity);
            context.SaveChanges();

            return MapToContract(entity);
        }

        public SpelResponseContract? GetById(int id)
        {
            var entity = context.Spels.Find(id);
            return entity is null ? null : MapToContract(entity);
        }
        private static SpelResponseContract MapToContract(Spel entity)
        {
            return new SpelResponseContract()
            {
                SpelId = entity.SpelId,
                SpeeldagId = entity.SpeeldagId,
                Terrein = entity.Terrein
            };
        }
    }
}
