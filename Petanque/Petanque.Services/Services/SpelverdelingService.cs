using Microsoft.EntityFrameworkCore;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;

namespace Petanque.Services.Services;

public class SpelverdelingService(Id312896PetanqueContext context) : ISpelverdelingService
{
    public SpelverdelingResponseContract GetById(int id)
    {
        var entity = context.Spelverdelings
                   .Include(s => s.Speler)
                   .FirstOrDefault(s => s.SpelverdelingsId == id);

        return entity is null ? null : MapToContract(entity);
    }
    
    private static SpelverdelingResponseContract MapToContract(Spelverdeling entity) 
    {
        return new SpelverdelingResponseContract
        {
            SpelverdelingsId = entity.SpelverdelingsId,
            SpelId = entity.SpelId,
            Team = entity.Team,
            SpelerPositie = entity.SpelerPositie,
            SpelerVolgnr = entity.SpelerVolgnr,
            Speler = new PlayerResponseContract {
                SpelerId = entity.Speler.SpelerId,
                Voornaam = entity.Speler.Voornaam,
                Naam = entity.speler.Naam
            }
        };
    }
}