using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;

namespace Petanque.Services.Interfaces;

public interface IPlayerService
{
    PlayerResponseContract? GetById(int id);
    PlayerResponseContract Create(PlayerRequestContract request);
}