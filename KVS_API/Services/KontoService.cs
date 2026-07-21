using KVS_API.Models;
using KVS_API.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KVS_API.Services;

public class KontoService : IKontoService
{
    private readonly ApplicationDbContext _context;
    private readonly IKontobewegungService _bewegungService;

    public KontoService(ApplicationDbContext context, IKontobewegungService bewegungService)
    {
        _context = context;
        _bewegungService = bewegungService;
    }

    public async Task<IEnumerable<KontoResponse>> GetAlleKontenAsync(Guid userId)
    {
        var kontenliste = await _context.Konten.Where(k => k.UserId == userId).ToListAsync();

        var responseliste = kontenliste.Select(k =>
            new KontoResponse(
                k.Kontonummer,
                k.Typ,
                k.GetSaldo(),
                k.Erstelltam
            )
        );

        return responseliste;
    }

    public async Task<KontoResponse> GetKontoAsync(string kontonummer, Guid aktuellerUserId)
    {
        var konto = await _context.Konten.Where(k => k.Kontonummer == kontonummer).FirstOrDefaultAsync();

        if (konto == null || konto.UserId != aktuellerUserId)
        {
            throw new KontoNotFoundException($"Das Konto mit der Kontonummer {kontonummer} existiert nicht.");
        }

        var responsekonto = new KontoResponse(
            konto.Kontonummer,
            konto.Typ,
            konto.GetSaldo(),
            konto.Erstelltam
        );

        return responsekonto;
    }

    public async Task<KontoResponse> ErstelleSparkontoAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            throw new UserNotFoundException($"User mit ID '{userId}' nicht gefunden.");
        }

        var anzahlKonten = await _context.Konten.CountAsync(k => k.UserId == userId);

        if (anzahlKonten >= 6)
        {
            throw new MaxKontenErreichtException("Max Konten erreicht");
        }

        var sparkonto = new Sparkonto();

        user.KontoHinzufuegen(sparkonto);

        await _context.SaveChangesAsync();

        return new KontoResponse(
            sparkonto.Kontonummer,
            sparkonto.Typ,
            sparkonto.GetSaldo(),
            sparkonto.Erstelltam
        );
    }

    public async Task<KontoResponse> ErstellePrivatkontoAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            throw new UserNotFoundException($"User mit ID '{userId}' nicht gefunden.");
        }

        var anzahlKonten = await _context.Konten.CountAsync(k => k.UserId == userId);

        if (anzahlKonten >= 6)
        {
            throw new MaxKontenErreichtException("Max Konten erreicht");
        }

        var privatkonto = new Privatkonto();

        user.KontoHinzufuegen(privatkonto);

        await _context.SaveChangesAsync();

        return new KontoResponse(
            privatkonto.Kontonummer,
            privatkonto.Typ,
            privatkonto.GetSaldo(),
            privatkonto.Erstelltam
        );

    }

    public async Task<KontoResponse> EinzahlenAsync(string kontonummer, decimal betrag, Guid aktuellerUserId)
    {
        var konto = await _context.Konten.Where(k => k.Kontonummer == kontonummer).FirstOrDefaultAsync();

        if (konto == null || konto.UserId != aktuellerUserId)
        {
            throw new KontoNotFoundException($"Das Konto mit der Kontonummer {kontonummer} existiert nicht.");
        }

        if (betrag <= 0)
        {
            throw new UngueltigerBetragException("Der Betrag muss größer als 0 sein.");
        }

        konto.Einzahlen(betrag);

        // Bewegung protokollieren (wird zusammen mit der Saldo-Aenderung gespeichert)
        _bewegungService.Erfasse(
            konto.Kontonummer,          // welches Konto
            BewegungsTyp.Einzahlung,    // Art
            betrag,                     // Betrag
            konto.GetSaldo(),           // Saldo NACH der Buchung
            aktuellerUserId);           // wer

        await _context.SaveChangesAsync();

        return new KontoResponse(
            konto.Kontonummer,
            konto.Typ,
            konto.GetSaldo(),
            konto.Erstelltam
        );
    }

    public async Task<KontoResponse> AuszahlenAsync(string kontonummer, decimal betrag, Guid aktuellerUserId)
    {
        var konto = await _context.Konten.Where(k => k.Kontonummer == kontonummer).FirstOrDefaultAsync();

        if (konto == null || konto.UserId != aktuellerUserId)
        {
            throw new KontoNotFoundException($"Das Konto mit der Kontonummer {kontonummer} existiert nicht.");
        }

        if (betrag <= 0)
        {
            throw new UngueltigerBetragException("Der Betrag muss größer als 0 sein.");
        }

        if (betrag > konto.GetSaldo())
        {
            throw new UnzureichendeDeckungException("Der Betrag muss kleiner als oder gleich dem Saldo sein.");
        }

        konto.Auszahlen(betrag);

        await _context.SaveChangesAsync();

        return new KontoResponse(
            konto.Kontonummer,
            konto.Typ,
            konto.GetSaldo(),
            konto.Erstelltam
        );
    }

    public async Task<UmbuchungResponse> UmbuchenAsync(string vonKontonummer, string nachKontonummer, decimal betrag, Guid aktuellerUserId)
    {
        var vonkonto = await _context.Konten.Where(k => k.Kontonummer == vonKontonummer).FirstOrDefaultAsync();
        var nachkonto = await _context.Konten.Where(k => k.Kontonummer == nachKontonummer).FirstOrDefaultAsync();

        if (vonkonto == null || nachkonto == null
            || vonkonto.UserId != aktuellerUserId || nachkonto.UserId != aktuellerUserId)
        {
            throw new KontoNotFoundException("Eines der Konten wurde nicht gefunden.");
        }

        var user = await _context.Users.FindAsync(aktuellerUserId);

        if (user == null)
        {
            throw new UserNotFoundException($"User mit ID '{vonkonto.UserId}' nicht gefunden.");
        }

        user.Umbuchen(vonkonto, nachkonto, betrag);
        await _context.SaveChangesAsync();

        return new UmbuchungResponse(
            Erfolgreich: true,
            Nachricht: $"Umbuchung von {betrag} CHF erfolgreich.",
            NeuerSaldoVon: vonkonto.GetSaldo(),
            NeuerSaldoNach: nachkonto.GetSaldo()
        );
    }

    public async Task<KontoResponse> MonatlicheAbrechnungAsync(string kontonummer)
    {
        var konto = await _context.Konten
        .Where(k => k.Kontonummer == kontonummer)
        .FirstOrDefaultAsync();

        if (konto == null)
        {
            throw new KontoNotFoundException($"Das Konto mit der Kontonummer {kontonummer} existiert nicht.");
        }
        konto.MonatlicheAbrechnung();

        await _context.SaveChangesAsync();

        return new KontoResponse(
            konto.Kontonummer,
            konto.Typ,
            konto.GetSaldo(),
            konto.Erstelltam
        );
    }

    public async Task KontoEntfernen(string kontonummer, Guid aktuellerUserId)
    {
        var konto = await _context.Konten
        .Where(k => k.Kontonummer == kontonummer)
        .FirstOrDefaultAsync();

        if (konto == null || konto.UserId != aktuellerUserId)
        {
            throw new KontoNotFoundException($"Das Konto mit der Kontonummer {kontonummer} existiert nicht.");
        }

        _context.Konten.Remove(konto);
        await _context.SaveChangesAsync();

    }
}