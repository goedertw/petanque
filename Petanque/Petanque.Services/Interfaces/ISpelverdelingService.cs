using Petanque.Contracts.Responses;
using static Petanque.Contracts.Responses.SpelverdelingSpellenResponseContract;

namespace Petanque.Services.Interfaces;

public interface ISpelverdelingService
{
    SpelverdelingResponseContract GetById(int id);
    List<SpelVerdelingRonde> GenereerVerdeling(List<string> spelers, int aantalRondes, int aantalSpeelterreinen);

}