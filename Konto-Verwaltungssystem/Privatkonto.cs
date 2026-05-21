using System;
using System.Collections.Generic;
using System.Text;

namespace Konto_Verwaltungssystem
{
    internal class Privatkonto : Konto
    {
        private decimal _saldo;

        public Privatkonto(string kontonummer) : base(kontonummer) 
        {
            this._saldo = GetSaldo();
        }

        public override void MonatlicheAbrechnung()
        {
            decimal gebuehr = this._saldo * 0.025m;
            this._saldo -= gebuehr;
        }
    }
}
