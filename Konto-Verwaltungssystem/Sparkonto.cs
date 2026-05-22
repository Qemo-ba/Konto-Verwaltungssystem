using System;
using System.Collections.Generic;
using System.Text;

namespace Konto_Verwaltungssystem
{
    internal class Sparkonto : Konto
    {
        public Sparkonto(string kontonummer) : base(kontonummer) { }

        public override void MonatlicheAbrechnung() 
        {
            decimal gebuehr = GetSaldo() * 0.025m;
            Einzahlen(gebuehr);
        }
        
    }
}
