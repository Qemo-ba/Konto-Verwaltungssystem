using KVS_API.Models;
using KVS_API.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace KVS_API.Services;

public class KontobewegungService : IKontobewegungService
{
    private readonly ApplicationDbContext _context;

    public KontobewegungService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Erfasse(
        string kontonummer,
        BewegungsTyp typ,
        decimal betrag,
        decimal saldoNachher,
        Guid ausgefuehrtVon,
        string? gegenKonto = null)
    {
        var bewegung = new Kontobewegung(kontonummer, typ, betrag, saldoNachher, ausgefuehrtVon, gegenKonto);
        _context.Kontobewegungen.Add(bewegung);
    }

    public async Task<IEnumerable<KontobewegungResponse>> GetBewegungenAsync(string kontonummer, Guid aktuellerUserId)
    {
        var konto = await _context.Konten.FirstOrDefaultAsync(k => k.Kontonummer == kontonummer);
        if (konto == null || konto.UserId != aktuellerUserId)
        {
            throw new KontoNotFoundException($"Das Konto mit der Kontonummer {kontonummer} existiert nicht.");
        }

        var bewegungenliste = await _context.Kontobewegungen
            .Where(b => b.Kontonummer == kontonummer)
            .OrderByDescending(b => b.Zeitpunkt)
            .ToListAsync();

        return bewegungenliste.Select(b => new KontobewegungResponse(
            b.Typ.ToString(),
            b.Betrag,
            b.SaldoNachher,
            b.GegenKonto,
            b.Zeitpunkt));
    }
}
