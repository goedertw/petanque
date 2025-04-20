using Petanque.Contracts.Responses;

public interface ISpelverdelingService
{
    SpelverdelingResponseContract GetById(int id);
    IEnumerable<SpelverdelingResponseContract> MaakVerdeling(IEnumerable<AanwezigheidResponseContract> aanwezigheden);
}
