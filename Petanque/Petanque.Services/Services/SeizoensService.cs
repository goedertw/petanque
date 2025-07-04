using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;

namespace Petanque.Services.Services;

public class SeizoensService : ISeizoensService
{
    private readonly Id312896PetanqueContext context;

    public SeizoensService(Id312896PetanqueContext context)
    {
        this.context = context;
    }
    public IEnumerable<SeizoenResponseContract> GetAll()
    {
        return context.Seizoens
            .OrderByDescending(s => s.Startdatum) // Meest recente seizoenen eerst
            .Select(s => MapToContract(s))
            .ToList();
    }

    public SeizoenResponseContract Create(SeizoenRequestContract request)
    {
        var entity = new Seizoen
        {
            Startdatum = request.Startdatum,
            Einddatum = request.Einddatum
        };
        
        var overlappingSeizoen = context.Seizoens.FirstOrDefault(s =>
            request.Startdatum <= s.Einddatum &&
            request.Einddatum >= s.Startdatum
        );

        if (overlappingSeizoen != null)
        {
            throw new InvalidOperationException($"Er bestaat al een seizoen dat overlapt met deze periode, namelijk seizoen {overlappingSeizoen.SeizoensId} ({overlappingSeizoen.Startdatum:dd/MM/yyyy}-{overlappingSeizoen.Einddatum:dd/MM/yyyy})");
        }

        context.Seizoens.Add(entity);
        context.SaveChanges();

        return MapToContract(entity);
    }

    private static SeizoenResponseContract MapToContract(Seizoen entity)
    {
        return new SeizoenResponseContract
        {
            SeizoensId = entity.SeizoensId,
            Startdatum = entity.Startdatum,
            Einddatum = entity.Einddatum,
        };
    }
}