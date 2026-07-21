using KVS_API.Models;

namespace KVS_API.Services;

public interface IKontobewegungService
{
    void Erfasse(
        string kontonummer,
        BewegungsTyp typ,
        decimal betrag,
        decimal saldoNachher,
        Guid ausgefuehrtVon,
        string? gegenKonto = null);

    Task<IEnumerable<KontobewegungResponse>> GetBewegungenAsync(string kontonummer, Guid aktuellerUserId);
}
