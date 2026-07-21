using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KVS_API.Models
{
    public enum BewegungsTyp
    {
        Einzahlung,
        Auszahlung,
        UmbuchungEingang,
        UmbuchungAusgang,
        Eroeffnung,
        Monatlicheabrechnung
    }
    public class Kontobewegung
    {
        [Key]
        [Column("id")]
        public Guid Id { get; private set; } = Guid.NewGuid();

        [Column("kontonummer")]
        public string Kontonummer { get; private set; }

        [Column("typ")]
        public BewegungsTyp Typ { get; private set; }

        [Column("betrag")]
        public decimal Betrag { get; private set; }

        [Column("saldonachher")]
        public decimal SaldoNachher { get; private set; }

        [Column("gegenkonto")]
        public string? GegenKonto { get; private set; }

        [Column("ausgefuehrtvon")]
        public Guid AusgefuehrtVon { get; private set; }

        [Column("zeitpunkt")]
        public DateTime Zeitpunkt { get; private set; } = DateTime.UtcNow;

        [Column("beschreibung")]
        public string? Beschreibung { get; private set; }

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
