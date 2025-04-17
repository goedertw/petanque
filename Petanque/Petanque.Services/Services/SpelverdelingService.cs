using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;

namespace Petanque.Services.Services;

public class SpelverdelingService(Id312896PetanqueContext context) : ISpelverdelingService
{
    public SpelverdelingResponseContract GetById(int id)
    {
        var spelverdeling = context.Spelverdelings
            .FirstOrDefault(sv => sv.SpelverdelingsId == id);

        if (spelverdeling == null)
            return null;

        var spel = context.Spels
            .FirstOrDefault(sp => sp.SpelId == spelverdeling.SpelId);

        var speeldagId = spel?.SpeeldagId;

        var aanwezigheid = context.Aanwezigheids
            .Include(a => a.Speler)
            .FirstOrDefault(a =>
                a.SpeeldagId == speeldagId &&
                a.SpelerVolgnr == spelverdeling.SpelerVolgnr);

        return MapToContract(spelverdeling, aanwezigheid?.Speler);
    }

    private static SpelverdelingResponseContract MapToContract(Spelverdeling entity, Speler? speler)
    {
        return new SpelverdelingResponseContract
        {
            SpelverdelingsId = entity.SpelverdelingsId,
            SpelId = entity.SpelId,
            Team = entity.Team,
            SpelerPositie = entity.SpelerPositie,
            SpelerVolgnr = entity.SpelerVolgnr,
            Speler = speler == null ? null : new PlayerResponseContract
            {
                SpelerId = speler.SpelerId,
                Voornaam = speler.Voornaam,
                Naam = speler.Naam
            }
        };
    }
}