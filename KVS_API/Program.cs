
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
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging()
            );

            builder.Services.AddHostedService<KVS_API.BackgroundServices.AbrechnungsService>();
            builder.Services.AddScoped<IKontoService, KontoService>();
            builder.Services.AddExceptionHandler<KVS_API.Middlewares.GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ErlaubeFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("ErlaubeFrontend");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
