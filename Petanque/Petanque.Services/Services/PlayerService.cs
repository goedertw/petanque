using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;

namespace Petanque.Services.Services;

public class PlayerService(Id312896PetanqueContext context) : IPlayerService
{
    public PlayerResponseContract Create(PlayerRequestContract request)
    {
        var entity = new Speler()
        {
            Voornaam = request.Voornaam,
            Naam = request.Naam
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
            Naam = entity.Naam
        };
    }
}