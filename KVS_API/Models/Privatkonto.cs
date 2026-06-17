using System;
using System.Collections.Generic;
using System.Text;

namespace KVS_API.Models
{
    public class Privatkonto : Konto
    {

        public Privatkonto() : base("Privatkonto", 50)
        {
        }

        public override void MonatlicheAbrechnung()
        {
            decimal gebuehr = GetSaldo() * 0.025m;
            Auszahlen(gebuehr);
        }
    }
}
