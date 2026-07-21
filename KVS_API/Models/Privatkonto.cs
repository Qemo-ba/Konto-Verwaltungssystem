

namespace KVS_API.Models
{
    public class Privatkonto : Konto
    {

        public Privatkonto() : base("Privatkonto", 50)
        {
        }

        public override decimal MonatlicheAbrechnung()
        {
            decimal gebuehr = GetSaldo() * 0.025m;
            Auszahlen(gebuehr);
            return gebuehr;
        }
    }
}
