using System;
using System.Collections.Generic;
using System.Text;

namespace KVS_API.Models
{
    public class Sparkonto : Konto
    {
        public Sparkonto() : base("Sparkonto", 50) { }

        public override void MonatlicheAbrechnung()
        {
            decimal gebuehr = GetSaldo() * 0.025m;
            Einzahlen(gebuehr);
        }

    }
}
