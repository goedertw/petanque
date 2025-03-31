using Petanque.Contracts;

namespace Petanque.Services;

public interface ISpelverdelingService
{
    SpelverdelingResponseContract GetSpelverdelingById(int id);
}