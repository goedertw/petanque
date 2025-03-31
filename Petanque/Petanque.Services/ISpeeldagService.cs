namespace Petanque.Services;

using Petanque.Contracts;
public interface ISpeeldagService
{
    SpeeldagResponseContract Create(SpeeldagRequestContract request);
}