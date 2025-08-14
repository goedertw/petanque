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
    public IEnumerable<PlayerResponseContract> GetAll()
    {
        return context.Spelers.OrderBy(a => a.Naam).ThenBy(a => a.Voornaam).Select(a => MapToContract(a)).ToList();
    }

    public void Update(int id, string voornaam, string naam)
    {
        var entity = context.Spelers.Find(id);
        if (entity is null)
        {
            throw new ArgumentException($"Lid met ID {id} werd niet gevonden");
        }
        entity.Voornaam = voornaam;
        entity.Naam = naam;
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var entity = context.Spelers.Find(id);
        if (entity is null)
        {
            throw new ArgumentException($"Lid met ID {id} werd niet gevonden");
        }
        context.Spelers.Remove(entity);
        context.SaveChanges();
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