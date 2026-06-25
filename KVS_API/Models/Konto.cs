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
        private decimal _saldo = 0;

        public virtual User User { get; set; }

        [Column("userid")]
        public Guid UserId { get; set; }

        [Column("typ")]
        public string Typ { get; private set; }
        [Column("erstelltam")]
        public DateTime Erstelltam { get; private set; } = DateTime.Now;

        protected Konto(string typ, decimal anfangsbestand)
        {
            this.Typ = typ;
            this.Kontonummer = GetKontonummer();
            if (anfangsbestand >= 0)
            {
                this._saldo = anfangsbestand;
            }
        }

        public void Einzahlen(decimal betrag)
        {
            if (betrag > 0)
            {
                _saldo += betrag;
            }
            else
            {
                throw new UngueltigerBetragException("Der Betrag ist ungültig");
            }
        }

        public bool Auszahlen(decimal betrag)
        {
            if (betrag > 0 && betrag <= _saldo)
            {
                _saldo -= betrag;
                return true;
            }
            else
            {
                return false;
            }
        }

        public decimal GetSaldo()
        {
            return _saldo;
        }

        private static string GetKontonummer()
        {
            var region = new RegionInfo(CultureInfo.InstalledUICulture.Name);
            var kontonummer = region.TwoLetterISORegionName + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            return kontonummer;
        }

        public abstract void MonatlicheAbrechnung();



    }
}
