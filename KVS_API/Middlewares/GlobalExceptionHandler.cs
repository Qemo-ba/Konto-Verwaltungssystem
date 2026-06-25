using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KVS_API.Exceptions;

namespace KVS_API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // 1. Den Fehler in der Konsole loggen (für dich als Entwickler wichtig)
            _logger.LogError(exception, "Ein Fehler ist aufgetreten: {Message}", exception.Message);

            // 2. Ein standardisiertes Fehler-Objekt vorbereiten
            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path
            };

            // 3. Fehler je nach Typ in die passenden HTTP-Statuscodes übersetzen
            if (exception is KontoNotFoundException || exception is UserNotFoundException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails.Title = "Nicht gefunden";
                problemDetails.Detail = exception.Message;
                problemDetails.Status = StatusCodes.Status404NotFound;
            }
            else if (exception is UngueltigerBetragException || exception is UnzureichendeDeckungException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Ungültige Anfrage";
                problemDetails.Detail = exception.Message;
                problemDetails.Status = StatusCodes.Status400BadRequest;
            }
            else
            {
                // Für alle anderen "echten" Abstürze (z. B. Datenbank down)
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Interner Serverfehler";
                problemDetails.Detail = "Ein unerwarteter Fehler ist aufgetreten.";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
            }

            // 4. Die saubere JSON-Antwort an das Frontend (Angular) schicken
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // Signalisiert .NET, dass wir den Fehler erfolgreich abgefangen haben
        }
    }
}