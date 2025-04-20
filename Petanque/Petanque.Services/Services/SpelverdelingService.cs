using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;

namespace Petanque.Services.Services;

public class SpelverdelingService(Id312896PetanqueContext context) : ISpelverdelingService
{
    public IEnumerable<SpelverdelingResponseContract> GetById(int speeldagId)
    {
        // Haal alle spellen op van de speeldag
        var spellen = context.Spels
            .Where(sp => sp.SpeeldagId == speeldagId)
            .ToList();

        if (!spellen.Any())
            return Enumerable.Empty<SpelverdelingResponseContract>();

        // Haal alle spelverdelingen voor deze spellen op
        var spelIds = spellen.Select(s => s.SpelId).ToList();

        var spelverdelingen = context.Spelverdelings
            .Where(sv => spelIds.Contains(sv.SpelId ?? 0))
            .ToList();

        // Haal aanwezigheid + speler info op
        var aanwezigheden = context.Aanwezigheids
            .Include(a => a.Speler)
            .Where(a => a.SpeeldagId == speeldagId)
            .ToList();

        return spelverdelingen.Select(sv =>
        {
            var speler = aanwezigheden
                .FirstOrDefault(a => a.SpelerVolgnr == sv.SpelerVolgnr)
                ?.Speler;

            var spel = spellen.FirstOrDefault(sp => sp.SpelId == sv.SpelId);

            return MapToContract(sv, speler, spel);
        }).ToList();
    }

    private static SpelverdelingResponseContract MapToContract(Spelverdeling entity, Speler? speler, Spel? spel)
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
            },
            Spel = spel == null ? null : new SpelResponseContract
            {
                SpelId = spel.SpelId,
                SpeeldagId = spel.SpeeldagId,
                Terrein = spel.Terrein
            }
        };
    }
}
