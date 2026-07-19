# Konto-Verwaltungssystem
Konto-Verwaltungssystem (KVS): Ein objektorientiertes C#-Backend, das Kernbanken-Prozesse simuliert. Fokus auf sichere Datenkapselung, Vererbungshierarchien und robuste Transaktionslogik über eine Konsolen-Schnittstelle.

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
