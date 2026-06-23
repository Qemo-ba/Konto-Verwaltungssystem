using KVS_API.Models;

namespace KVS_API.Services;

public interface IKontoService
{
    Task<IEnumerable<KontoResponse>> GetAlleKontenAsync(Guid userId);
    Task<KontoResponse> GetKontoAsync(string kontonummer);

    Task<KontoResponse> ErstelleSparkontoAsync(Guid userId);
    Task<KontoResponse> ErstellePrivatkontoAsync(Guid userId);

    Task<KontoResponse> EinzahlenAsync(string kontonummer, decimal betrag);
    Task<KontoResponse> AuszahlenAsync(string kontonummer, decimal betrag);
    Task<UmbuchungResponse> UmbuchenAsync(string vonKontonummer, string nachKontonummer, decimal betrag);

    Task<KontoResponse> MonatlicheAbrechnungAsync(string kontonummer);

}