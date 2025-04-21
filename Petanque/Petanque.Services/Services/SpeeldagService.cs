using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;

namespace Petanque.Services.Services;

public class SpeeldagService(Id312896PetanqueContext context) : ISpeeldagService
{
    public SpeeldagResponseContract Create(SpeeldagRequestContract request)
    {
        var speeldag = new Speeldag
        {
            Datum = request.Datum,
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
        return MapToContract(entity);
    }

    public IEnumerable<SpeeldagResponseContract> GetAll()
    {
        return context.Speeldags
        .Include(s => s.Seizoens)
        .Include(s => s.Spels)
        .AsEnumerable() 
        .Select(a => MapToContract(a))
        .ToList();
    }

    private SpeeldagResponseContract MapToContract(Speeldag entity)
    {
        return new SpeeldagResponseContract
        {
            SpeeldagId = entity.SpeeldagId,
            Datum = entity.Datum,
            Spel = (List<SpelResponseContract>)entity.Spels
            .Where(s => s.SpeeldagId == entity.SpeeldagId)
            .Select(s => new SpelResponseContract
            {
                SpelId = s.SpelId,
                SpeeldagId = s.SpeeldagId,
                Terrein = s.Terrein,
                ScoreA = s.ScoreA,
                ScoreB = s.ScoreB,
                Spelverdelingen = context.Spelverdelings
                    .Where(sv => sv.SpelId == s.SpelId)
                    .Select(sv => new SpelverdelingResponseContract
                    {
                        SpelverdelingsId = sv.SpelverdelingsId,
                        SpelId = sv.SpelId,
                        Team = sv.Team,
                        SpelerVolgnr = sv.SpelerVolgnr
                    })
                    .ToList()
            }).ToList()
        };
    }


}