using Petanque.Contracts.Responses;

public interface ISpelverdelingService
{
    IEnumerable<SpelverdelingResponseContract> GetById(int id);
    IEnumerable<SpelverdelingResponseContract> MaakVerdeling(IEnumerable<AanwezigheidResponseContract> aanwezigheden, int id);
    IEnumerable<SpelverdelingResponseContract> GetBySpeeldagAndTerrein(int speeldag, int terrein);
}
