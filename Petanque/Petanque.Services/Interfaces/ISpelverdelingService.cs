using Petanque.Contracts.Responses;

namespace Petanque.Services.Interfaces;

public interface ISpelverdelingService
{
    IEnumerable<SpelverdelingResponseContract> GetById(int id);
}