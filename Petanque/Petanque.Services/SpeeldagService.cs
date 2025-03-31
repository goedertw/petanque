using Petanque.Contracts;
using Petanque.Storage;

namespace Petanque.Services;

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
    
    private SpeeldagResponseContract MapToContract(Speeldag speeldag)
    {
        return new SpeeldagResponseContract
        {
            SpeeldagId = speeldag.SpeeldagId,
            Datum = speeldag.Datum,
            SeizoensId = speeldag.SeizoensId,
        };
    }
}