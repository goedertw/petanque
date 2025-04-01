using Petanque.Contracts.Responses;

namespace Petanque.Services.Interfaces;

public interface ISpelverdelingService
{
    SpelverdelingResponseContract GetById(int id);
}