using System;
using System.Collections.Generic;
using System.Text;

namespace Konto_Verwaltungssystem
{
    internal class Privatkonto : Konto
    {

        public Privatkonto(string kontonummer) : base(kontonummer) 
        {
        }

        public override void MonatlicheAbrechnung()
        {
            decimal gebuehr = GetSaldo() * 0.025m;
            Auszahlen(gebuehr);
        }
    }
}
