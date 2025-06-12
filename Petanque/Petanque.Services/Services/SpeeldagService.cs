using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using System.Collections.Generic;
using System.Linq;

namespace Petanque.Services.Services
{
    public class SpeeldagService : ISpeeldagService
    {
        private readonly Id312896PetanqueContext context;

        public SpeeldagService(Id312896PetanqueContext context)
        {
            this.context = context;
        }

        public SpeeldagResponseContract Create(SpeeldagRequestContract request)
        {
            var speeldagCheck = context.Speeldags.FirstOrDefault(sd => sd.Datum == request.Datum);

            if (speeldagCheck != null)
                return MapToContract(speeldagCheck);

            var speeldag = new Speeldag
            {
                Datum = request.Datum,
                SeizoensId = request.SeizoensId
            };

            context.Speeldags.Add(speeldag);
            context.SaveChanges();

            // Gebruik de versie van MapToContract zonder spelverdelingen
            return MapToContract(speeldag);
        }

        public SpeeldagResponseContract GetById(int id)
        {
            var entity = context.Speeldags
                .Include(s => s.Seizoens)
                .Include(s => s.Spels)
                .FirstOrDefault(s => s.SpeeldagId == id);

            if (entity == null)
                return null;

            var spelIds = entity.Spels.Select(s => s.SpelId).ToList();

            // Belangrijk: Include Speler zodat spelerinformatie beschikbaar is
            var spelverdelingen = context.Spelverdelings
                .Where(sv => spelIds.Contains((int)sv.SpelId))
                .Include(sv => sv.Speler)
                .ToList();

            return MapToContract(entity, spelverdelingen);
        }

        public IEnumerable<SpeeldagResponseContract> GetAll()
        {
            var speeldagen = context.Speeldags
                                    .Include(s => s.Seizoens)
                                    .Include(s => s.Spels)
                                    .ToList();

            var spelIds = speeldagen.SelectMany(s => s.Spels).Select(sp => sp.SpelId).Distinct().ToList();

            var spelverdelingen = context.Spelverdelings
                                         .Include(sv => sv.Speler)
                                         .Where(sv => spelIds.Contains((int)sv.SpelId))
                                         .ToList();

            return speeldagen
                .Select(a => MapToContract(a, spelverdelingen))
                .ToList();
        }

        private SpeeldagResponseContract MapToContract(Speeldag entity)
        {
            return new SpeeldagResponseContract
            {
                SpeeldagId = entity.SpeeldagId,
                Datum = entity.Datum,
                Spel = entity.Spels
                    .Select(s => new SpelResponseContract
                    {
                        SpelId = s.SpelId,
                        SpeeldagId = s.SpeeldagId,
                        Terrein = s.Terrein,
                        ScoreA = s.ScoreA,
                        ScoreB = s.ScoreB,
                        Spelverdelingen = new List<SpelverdelingResponseContract>()
                    })
                    .ToList()
            };
        }

        private SpeeldagResponseContract MapToContract(Speeldag entity, List<Spelverdeling> spelverdelingen)
        {
            return new SpeeldagResponseContract
            {
                SpeeldagId = entity.SpeeldagId,
                Datum = entity.Datum,
                Spel = entity.Spels
                    .Select(s => new SpelResponseContract
                    {
                        SpelId = s.SpelId,
                        SpeeldagId = s.SpeeldagId,
                        Terrein = s.Terrein,
                        ScoreA = s.ScoreA,
                        ScoreB = s.ScoreB,
                        Spelverdelingen = spelverdelingen
                            .Where(sv => sv.SpelId == s.SpelId)
                            .Select(sv => new SpelverdelingResponseContract
                            {
                                SpelverdelingsId = sv.SpelverdelingsId,
                                SpelId = sv.SpelId,
                                Team = sv.Team,
                                SpelerVolgnr = sv.SpelerVolgnr,
                                Speler = sv.Speler == null ? null : new PlayerResponseContract
                                {
                                    SpelerId = sv.Speler.SpelerId,
                                    Voornaam = sv.Speler.Voornaam,
                                    Naam = sv.Speler.Naam
                                }
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }
}
