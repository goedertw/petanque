namespace Petanque.Services;

public interface IPlayerService
{
    PlayerResponseContract? GetById(int id);
    PlayerResponseContract Create(PlayerRequestContract request);
}