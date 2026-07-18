using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using KVS_API.Exceptions;

namespace KVS_API.Models
{
    public abstract class Konto
    {
        [Key]
        [Column("kontonummer")]
        public string Kontonummer { get; private set; }

        [Column("saldo")]
        public decimal Saldo { get; private set; } = 0;

        public virtual User User { get; set; }

        [Column("userid")]
        public Guid UserId { get; set; }

        [Column("typ")]
        public string Typ { get; private set; }
        [Column("erstelltam")]
        public DateTime Erstelltam { get; private set; } = DateTime.UtcNow;

        [Column("letzteabrechnung")]
        public DateTime LetzteAbrechnung { get; set; } = DateTime.UtcNow;

        protected Konto(string typ, decimal anfangsbestand)
        {
            this.Typ = typ;
            this.Kontonummer = GetKontonummer();
            if (anfangsbestand >= 0)
            {
                this.Saldo = anfangsbestand;
            }
        }

        public void Einzahlen(decimal betrag)
        {
            if (betrag > 0)
            {
                Saldo += betrag;
            }
            else
            {
                throw new UngueltigerBetragException("Der Betrag ist ungültig");
            }
        }

        public bool Auszahlen(decimal betrag)
        {
            if (betrag > 0 && betrag <= Saldo)
            {
                Saldo -= betrag;
                return true;
            }
            else
            {
                return false;
            }
        }

        public decimal GetSaldo()
        {
            return Saldo;
        }

        private static string GetKontonummer()
        {
            string regionCode = "CH";
            try
            {
                var region = new RegionInfo(CultureInfo.InstalledUICulture.Name);
                regionCode = region.TwoLetterISORegionName;
            }
            catch { }

            var kontonummer = regionCode + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            return kontonummer;
        }

        public abstract void MonatlicheAbrechnung();



    }
}
