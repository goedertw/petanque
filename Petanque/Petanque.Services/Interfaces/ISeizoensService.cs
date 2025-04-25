namespace Petanque.Services.Interfaces;
using Petanque.Contracts.Responses;

public interface ISeizoensService
{
    IEnumerable<SeizoenResponseContract> GetAll();
}