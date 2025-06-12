using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
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
            var requestedDate = request.Datum.Date;

            var speeldagCheck = context.Speeldags
                .FirstOrDefault(sd => EF.Functions.DateDiffDay(sd.Datum, requestedDate) == 0);

            if (speeldagCheck != null)
                return MapToContract(speeldagCheck);

            var speeldag = new Speeldag
            {
                Datum = requestedDate,  // schrijf enkel de Date component weg
                SeizoensId = request.SeizoensId
            };

            context.Speeldags.Add(speeldag);
            context.SaveChanges();

            return MapToContract(speeldag);
        }


        public SpeeldagResponseContract GetById(int id)
        {
            var entity = context.Speeldags
                                .Include(s => s.Seizoens)
                                .Include(s => s.Spels)
                                .FirstOrDefault(s => s.SpeeldagId == id);

            if (entity == null)
            {
                return null;
            }

            // Haal de SpelId's van de betrokken Spels op, eerst in-memory
            var spelIds = entity.Spels.Select(s => s.SpelId).ToList();

            // Haal de spelverdelingen op voor de betrokken Spels op basis van de spelIds
            var spelverdelingen = context.Spelverdelings
                                         .Where(sv => spelIds.Contains((int)sv.SpelId))
                                         .ToList();

            // Gebruik de versie van MapToContract met spelverdelingen als parameter
            return MapToContract(entity, spelverdelingen);
        }

        public IEnumerable<SpeeldagResponseContract> GetAll()
        {
            var speeldagen = context.Speeldags
                                    .Include(s => s.Seizoens)
                                    .Include(s => s.Spels)
                                    .ToList(); // Haal de speeldagen en spels in-memory op

            // Haal alle spelverdelingen op die passen bij de spelId's van de opgehaalde spels
            var spelIds = speeldagen.SelectMany(s => s.Spels).Select(sp => sp.SpelId).Distinct().ToList();

            // Haal de spelverdelingen uit de database die overeenkomen met de spelIds
            var spelverdelingen = context.Spelverdelings
                                         .Where(sv => spelIds.Contains((int)sv.SpelId))
                                         .ToList();

            // Gebruik de versie van MapToContract met spelverdelingen als parameter
            return speeldagen
                .Select(a => MapToContract(a, spelverdelingen))
                .ToList();
        }

        // Versie van MapToContract zonder spelverdelingen, voor bijvoorbeeld Create
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
                        // Geen spelverdelingen hier, want die zijn niet meegegeven
                        Spelverdelingen = new List<SpelverdelingResponseContract>()
                    })
                    .ToList()
            };
        }

        // Versie van MapToContract met spelverdelingen als parameter
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
                                SpelerVolgnr = sv.SpelerVolgnr
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }
}
