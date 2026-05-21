using System;
using System.Collections.Generic;
using System.Text;

namespace Konto_Verwaltungssystem
{
    internal class Sparkonto : Konto
    {
        public Sparkonto(string kontonummer) : base(kontonummer) { }

        public override void MonatlicheAbrechnung() { }
        
    }
}
