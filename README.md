# Konto-Verwaltungssystem (KVS) – Backend / API

Objektorientiertes **C#-Backend**, das Kernbanken-Prozesse simuliert: Konten eröffnen,
ein- und auszahlen, umbuchen und monatlich abrechnen. Umgesetzt als **ASP.NET-Core-REST-API**
mit JWT-Authentifizierung, PostgreSQL-Persistenz (EF Core) und einem Hintergrunddienst für
die monatliche Abrechnung. Fokus auf saubere Datenkapselung, Vererbungshierarchien
(Privat-/Sparkonto) und nachvollziehbare Transaktionslogik.

> **Frontend:** [Qemo-ba/KVS](https://github.com/Qemo-ba/KVS) (Angular) · Live: https://kvs-murex.vercel.app
> **API-Host:** `https://konto-verwaltungssystem.onrender.com`

## Funktionen

- **Authentifizierung** – Registrierung & Login, Passwörter mit BCrypt gehasht, Ausgabe eines JWT
- **Konten** – Privatkonto und Sparkonto (Vererbung von einer abstrakten `Konto`-Basisklasse)
- **Transaktionen** – Einzahlen, Auszahlen (mit Deckungsprüfung), Umbuchen zwischen Konten
- **Kontobewegungen** – jede Transaktion wird als Bewegung protokolliert
- **Monatliche Abrechnung** – automatisiert über einen `BackgroundService`
- **Autorisierung** – jede Operation prüft, dass das Konto dem angemeldeten Benutzer gehört
- **Globales Error-Handling** – zentrale Exception-Middleware mit ProblemDetails

## Tech-Stack

| Bereich        | Technologie                                     |
| -------------- | ----------------------------------------------- |
| Sprache        | C# / .NET                                        |
| Framework      | ASP.NET Core Web API                             |
| Persistenz     | Entity Framework Core · PostgreSQL (Supabase)   |
| Auth           | JWT (HMAC-SHA256) · BCrypt                       |
| Deployment     | Docker · Render                                  |
| Tests          | xUnit (`KVS_API.Tests`)                          |

## Architektur

```
KVS_API/
├── Controllers/         # UserController (Auth), KontoController (Konten & Transaktionen)
├── Services/            # KontoService, KontobewegungService (+ Interfaces) – Geschäftslogik
├── Models/              # Domänenmodelle (Konto, Privatkonto, Sparkonto, User) + DTOs + DbContext
├── Backgroundservices/  # AbrechnungsService – monatliche Abrechnung
├── Middlewares/         # GlobalExceptionHandler
├── Exceptions/          # fachliche Exceptions (z. B. UnzureichendeDeckungException)
├── Migrations/          # EF-Core-Migrationen
└── Program.cs           # DI, Auth, CORS, Migration beim Start
```

Schichtung: **Controller → Service (Interface) → Domänenmodell → EF Core**.
Datenkapselung über `private set`, fachliche Regeln (Deckung, Kontolimit) in Domäne bzw. Service.

## Lokal starten

Voraussetzung: .NET SDK und eine PostgreSQL-Datenbank. Zuerst die Secrets setzen (siehe unten), dann:

```bash
cd KVS_API
dotnet run          # startet die API; Migrationen werden beim Start automatisch angewendet
```

Tests ausführen:

```bash
dotnet test
```

## Konfiguration / Secrets

Geheime Werte stehen **nicht** in `appsettings.json` (dort nur leere Platzhalter),
damit sie nicht im Git landen. Gesetzt werden müssen:

| Schlüssel                              | Beschreibung                                  |
| -------------------------------------- | --------------------------------------------- |
| `ConnectionStrings:DefaultConnection`  | PostgreSQL-Verbindungsstring (Supabase)       |
| `Jwt:Key`                              | Signaturschlüssel für JWT (langer Zufallswert)|

### Lokal (Entwicklung) – .NET User Secrets

```bash
dotnet user-secrets set "Jwt:Key" "<zufälliger-schlüssel>"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=postgres;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true"
```

### Produktiv (z. B. Render) – Umgebungsvariablen

Auf Servern werden Doppelpunkte als doppelte Unterstriche geschrieben:

```
Jwt__Key
ConnectionStrings__DefaultConnection
```
