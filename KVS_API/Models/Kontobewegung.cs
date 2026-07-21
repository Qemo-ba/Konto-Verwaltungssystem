using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVS_API.Models
{
    // Art einer Bewegung. Wird als lesbarer String in der DB gespeichert
    // (siehe ApplicationDbContext -> HasConversion<string>).
    public enum BewegungsTyp
    {
        Einzahlung,
        Auszahlung,
        UmbuchungEingang,
        UmbuchungAusgang,
        Eroeffnung
    }

    // Ein einzelner Protokoll-Eintrag ("wer/wann/was/wie viel/wohin").
    public class Kontobewegung
    {
        [Key]
        [Column("id")]
        public Guid Id { get; private set; } = Guid.NewGuid();

        // Konto, zu dem dieser Eintrag gehoert (FK -> konten.kontonummer)
        [Column("kontonummer")]
        public string Kontonummer { get; private set; }

        [Column("typ")]
        public BewegungsTyp Typ { get; private set; }

        // bewegter Betrag, immer positiv
        [Column("betrag")]
        public decimal Betrag { get; private set; }

        // Saldo nach der Buchung (praktisch fuer die Anzeige)
        [Column("saldonachher")]
        public decimal SaldoNachher { get; private set; }

        // bei Umbuchungen das andere Konto, sonst null
        [Column("gegenkonto")]
        public string? GegenKonto { get; private set; }

        // User-ID des Ausfuehrenden (aus dem JWT) - das "wer"
        [Column("ausgefuehrtvon")]
        public Guid AusgefuehrtVon { get; private set; }

        [Column("zeitpunkt")]
        public DateTime Zeitpunkt { get; private set; } = DateTime.UtcNow;

        [Column("beschreibung")]
        public string? Beschreibung { get; private set; }

        // Parameterloser Konstruktor nur fuer EF Core (Materialisierung).
        private Kontobewegung() { }

        public Kontobewegung(
            string kontonummer,
            BewegungsTyp typ,
            decimal betrag,
            decimal saldoNachher,
            Guid ausgefuehrtVon,
            string? gegenKonto = null,
            string? beschreibung = null)
        {
            Kontonummer = kontonummer;
            Typ = typ;
            Betrag = betrag;
            SaldoNachher = saldoNachher;
            AusgefuehrtVon = ausgefuehrtVon;
            GegenKonto = gegenKonto;
            Beschreibung = beschreibung;
        }
    }
}
