using KVS_API.Models;

namespace KVS_API.Services;

public interface IKontobewegungService
{
    // Erfasst eine Bewegung: fuegt sie nur dem DbContext hinzu.
    // Das eigentliche Speichern (SaveChanges) macht der Aufrufer (KontoService),
    // damit Buchung + Protokoll in EINER Transaktion landen.
    void Erfasse(
        string kontonummer,
        BewegungsTyp typ,
        decimal betrag,
        decimal saldoNachher,
        Guid ausgefuehrtVon,
        string? gegenKonto = null);

    // Liefert die Bewegungen eines Kontos (neueste zuerst) - mit Besitzer-Pruefung.
    Task<IEnumerable<KontobewegungResponse>> GetBewegungenAsync(string kontonummer, Guid aktuellerUserId);
}
