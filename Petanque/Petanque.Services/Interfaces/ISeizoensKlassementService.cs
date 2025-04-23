namespace Petanque.Services.Interfaces;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;

public interface ISeizoensKlassementService
{
    IEnumerable<SeizoensKlassementResponseContract> CreateSeizoensKlassementen(int seizoenId);
    IEnumerable<SeizoensKlassementResponseContract> GetById(int seizoenId);
}