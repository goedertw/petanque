using Petanque.Contracts;

namespace Petanque.Services;

public interface ISpelverdelingService
{
    SpelverdelingResponseContract GetById(int id);
}