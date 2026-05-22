using System;
using System.Collections.Generic;
using System.Text;

namespace Konto_Verwaltungssystem
{
    abstract class Konto
    {
        private decimal _saldo = 0;
        public string Kontonummer { get; private set; }

        public Konto(string kontonummer)
        {
            this.Kontonummer = kontonummer;
        }

        public void Einzahlen(decimal betrag)
        {
            if (betrag > 0)
            {
                _saldo += betrag;
            } else
            {
                Console.WriteLine("Warnung: Der Betrag ist negativ. Die Einzahlung wurde unterbrochen.");
            }
        }

        public void Auszahlen(decimal betrag)
        {
            if (betrag > 0 && betrag <= _saldo)
            {
                _saldo -= betrag;
            } else
            {
                Console.WriteLine("Warnung: Der Betrag ist negativ. Die Auszahlung wurde unterbrochen.");
            }
        }

        public decimal GetSaldo()
        {
            return _saldo;
        }

        public abstract void MonatlicheAbrechnung();

    }
}
