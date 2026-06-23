using System.ComponentModel.DataAnnotations.Schema;

namespace KVS_API.Models
{
    public class User
    {
        [Column("id")]
        public Guid Id { get; private set; }

        [Column("username")]
        public string Username { get; private set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("passwordhash")]
        private string Passwordhash;

        [Column("erstelltam")]
        public DateTime Erstelltam { get; private set; } = DateTime.Now;

        public ICollection<Konto> Konten { get; private set; } = new List<Konto>();

        public User(string username, string email, string passwordhash)
        {
            this.Username = username;
            this.Email = email;
            this.Passwordhash = passwordhash;
        }

        public void KontoHinzufuegen(Konto neuesKonto)
        {
            Konten.Add(neuesKonto);
        }

        public void Umbuchen(Konto vonkonto, Konto nachkonto, decimal betrag)
        {

            bool betragausbezahlt = vonkonto.Auszahlen(betrag);
            if (betragausbezahlt)
            {
                nachkonto.Einzahlen(betrag);
            }
            else
            {
                Console.WriteLine("Unbuchung wurde Unterbrochen");
            }

        }


    }
}
