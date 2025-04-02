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

    private SpeeldagResponseContract MapToContract(Speeldag entity)
    {
        return new SpeeldagResponseContract
        {
            SpeeldagId = entity.SpeeldagId,
            Datum = entity.Datum,
            Seizoenen = entity.Seizoens != null ? new SeizoenResponseContract
            {
               SeizoensId = entity.Seizoens.SeizoensId,
               Startdatum = entity.Seizoens.Startdatum,
               Einddatum = entity.Seizoens.Einddatum,

            } : null,
            Spellen = entity.Spels.Select(s => new SpelResponseContract
            {
                SpelId = s.SpelId,
                SpeeldagId= s.SpeeldagId,
                SpelerVolgnr = s.SpelerVolgnr,
                Terrein = s.Terrein,
                ScoreA = s.ScoreA,
                ScoreB = s.ScoreB
            }).ToList()
        };
    }
}