namespace Petanque.Services.Interfaces;

using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;

public interface ISpeeldagService
{
    SpeeldagResponseContract Create(SpeeldagRequestContract request);
    SpeeldagResponseContract GetById(int id);
}