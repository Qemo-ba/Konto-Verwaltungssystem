namespace Konto_Verwaltungssystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Privatkonto privatkonto = new Privatkonto("123456789");
            privatkonto.Einzahlen(1000);
            privatkonto.GetSaldo();
            privatkonto.MonatlicheAbrechnung();
            Console.WriteLine("Monatliche Abrechnung durchgeführt...");
            privatkonto.GetSaldo();
        }
    }
}
