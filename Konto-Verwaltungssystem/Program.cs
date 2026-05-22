namespace Konto_Verwaltungssystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Privatkonto privatkonto = new Privatkonto("priv123");
            Sparkonto sparkonto = new Sparkonto("spar123");
            List<Konto> konten = new List<Konto>();
            konten.Add(privatkonto);
            konten.Add(privatkonto);

            foreach (Konto konto in konten)
            {
                Console.WriteLine(konto.Kontonummer);
                konto.Einzahlen(1000);
                Console.WriteLine(konto.GetSaldo());
                konto.MonatlicheAbrechnung();
                Console.WriteLine(konto.GetSaldo());
                konto.Auszahlen(500);
                Console.WriteLine(konto.GetSaldo());
            }
            

        }
    }
}
