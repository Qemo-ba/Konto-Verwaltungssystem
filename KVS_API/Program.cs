
using KVS_API.Models;
using Microsoft.EntityFrameworkCore;
using KVS_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KVS_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();


            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
            });

            builder.Services.AddHostedService<KVS_API.BackgroundServices.AbrechnungsService>();
            builder.Services.AddScoped<IKontoService, KontoService>();
            builder.Services.AddScoped<IKontobewegungService, KontobewegungService>();
            builder.Services.AddExceptionHandler<KVS_API.Middlewares.GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ErlaubeFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200", "https://kvs-murex.vercel.app")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Fail-fast: Fehlt der JWT-Key (z. B. Render-Env-Var 'Jwt__Key' nicht gesetzt),
            // wuerde das Signieren spaeter mit einer kryptischen 500 scheitern. Lieber sofort
            // beim Start mit einer klaren Meldung abbrechen. HMAC-SHA256 braucht >= 16 Byte.
            var jwtKey = builder.Configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 16)
            {
                throw new InvalidOperationException(
                    "Jwt:Key fehlt oder ist zu kurz. Auf Render die Umgebungsvariable 'Jwt__Key' " +
                    "(doppelter Unterstrich!) auf einen langen Zufallswert (mind. 16 Zeichen) setzen.");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    // Dieser Befehl wendet alle fehlenden Migrationen automatisch auf Supabase an:
                    context.Database.Migrate();
                    Console.WriteLine("--> Datenbank-Migration erfolgreich durchgeführt!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Fehler bei der Datenbank-Migration: {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseExceptionHandler();
            //app.UseHttpsRedirection();

            app.UseCors("ErlaubeFrontend");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
