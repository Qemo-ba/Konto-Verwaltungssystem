namespace KVS_API.Models
{
    public record ErstelleKontoRequest(Guid UserId, string Typ);
    public record EinzahlenRequest(decimal Betrag);
    public record AuszahlenRequest(decimal Betrag);
    public record UmbuchenRequest(string VonKontonummer, string NachKontonummer, decimal Betrag);


    public record KontoResponse(string Kontonummer, string Typ, decimal Saldo, DateTime ErstelltAm);
    public record UmbuchungResponse(bool Erfolgreich, string Nachricht, decimal NeuerSaldoVon, decimal NeuerSaldoNach);
}
