using System.IO;
using Petanque.Contracts.Responses;

namespace Petanque.Services.Interfaces
{
    public interface ISpelverdelingPDFService
    {
        Stream GenerateSpelverdelingPDF(IEnumerable<SpelverdelingResponseContract> spelverdelingen);
    }
}
