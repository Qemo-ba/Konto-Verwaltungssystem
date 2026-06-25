using KVS_API.Models;
using Microsoft.EntityFrameworkCore;

namespace KVS_API.BackgroundServices
{
    // Die Klasse muss von BackgroundService erben!
    public class AbrechnungsService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AbrechnungsService> _logger;

        // Warum IServiceProvider? Ein BackgroundService läuft als "Singleton" (für immer). 
        // Die Datenbank (DbContext) existiert aber eigentlich nur kurz pro HTTP-Request. 
        // Mit dem ServiceProvider können wir uns selbst kurz eine Datenbank-Verbindung holen.
        public AbrechnungsService(IServiceProvider serviceProvider, ILogger<AbrechnungsService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Diese Methode wird automatisch gestartet, wenn die API hochfährt
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Der Abrechnungs-Service wurde im Hintergrund gestartet.");

            // Diese Schleife läuft, solange deine API eingeschaltet ist
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 1. Wir öffnen einen neuen "Scope" (eine neue Arbeitsumgebung)
                    using var scope = _serviceProvider.CreateScope();

                    // 2. Wir holen uns unsere Datenbank
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // 3. Wir holen alle Konten aus der Datenbank
                    var konten = await dbContext.Konten.ToListAsync(stoppingToken);

                    // 4. Für jedes Konto die von dir geschriebene Methode ausführen
                    foreach (var konto in konten)
                    {
                        konto.MonatlicheAbrechnung();
                    }

                    // 5. Alle Änderungen (neue Salden) in der DB speichern
                    await dbContext.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation($"Monatliche Abrechnung für {konten.Count} Konten erfolgreich durchgeführt.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fehler bei der monatlichen Abrechnung.");
                }

                await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
            }
        }
    }
}