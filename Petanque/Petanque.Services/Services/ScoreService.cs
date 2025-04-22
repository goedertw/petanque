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

namespace Petanque.Services.Services
{
    public class ScoreService(Id312896PetanqueContext context) : IScoreService
    {

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

        public void UpdateScore(int spelId, int scoreA, int scoreB) {
            var spel = context.Spels.Find(spelId);
            if (spel == null) throw new Exception("Spel niet gevonden");

            spel.ScoreA = scoreA;
            spel.ScoreB = scoreB;

            context.SaveChanges();
        }
    }
}
