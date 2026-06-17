using Microsoft.AspNetCore.Mvc;
using KVS_API.Models;

namespace KVS_API.Controllers
{
    [ApiController]
    [Route("api/konto")]
    public class KontoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public KontoController(ApplicationDbContext context) 
        {
            this._context = context;
        }

        [HttpGet("erstelle-user")]
        public IActionResult ErstelleUser()
        {
            User neuerUser = new User("Qemal");

            _context.Users.Add(neuerUser);
            _context.SaveChanges();

            return Ok(new
            {
                Info = "Erfolgreich in Supabase gespeichert!",
                UserId = neuerUser.Id,
                Username = neuerUser.Username
            });
        }

        [HttpGet("test-umbuchung")]
        public IActionResult TestUmbuchung()
        {
            User meinUser = new User("Qemal");
            Privatkonto privat = new Privatkonto();
            Sparkonto spar = new Sparkonto();

            meinUser.KontoHinzufuegen(privat);
            meinUser.KontoHinzufuegen(spar);

            privat.Einzahlen(1000);
            meinUser.Umbuchen(privat, spar, 300);

            return Ok(new
            {
                Kunde = meinUser.Username,
                Privatkonto_Nummer = privat.Kontonummer,
                Privatkonto_Saldo = privat.GetSaldo(),
                Sparkonto_Nummer = spar.Kontonummer,
                Sparkonto_Saldo = spar.GetSaldo()
            });
        }

        [HttpPost("privatkonto-eroeffnen/{userId}")]
        public IActionResult KontoEroeffnen(Guid userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("Kein User mit dieser ID gefunden!");
            }

            Privatkonto neuesKonto = new Privatkonto();

            neuesKonto.UserId = user.Id;

            _context.Konten.Add(neuesKonto);
            _context.SaveChanges();

            return Ok(new
            {
                Info = "Konto erfolgreich in Supabase erstellt!",
                Kontonummer = neuesKonto.Kontonummer,
                Besitzer = user.Username
            });
        }

        [HttpPost("sparkonto-eroeffnen/{userId}")]
        public IActionResult SparkontoEroeffnen(Guid userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("Kein User mit dieser ID gefunden!");
            }

            Sparkonto neuesKonto = new Sparkonto();

            neuesKonto.UserId = user.Id;

            _context.Konten.Add(neuesKonto);
            _context.SaveChanges();

            return Ok(new
            {
                Info = "Konto erfolgreich in Supabase erstellt!",
                Kontonummer = neuesKonto.Kontonummer,
                Besitzer = user.Username
            });
        }

        [HttpGet("meine-konten/{userId}")]
        public IActionResult HoleMeineKonten(Guid userId)
        {

            var userKonten = _context.Konten.Where(k => k.UserId == userId).ToList();

            if (userKonten.Count == 0)
            {
                return NotFound("Keine Konten für diesen User gefunden.");
            }
            return Ok(userKonten);
        }
    }
}
