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