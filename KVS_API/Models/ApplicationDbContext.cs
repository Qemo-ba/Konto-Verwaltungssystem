using Microsoft.EntityFrameworkCore;

namespace KVS_API.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Konto> Konten { get; set; }
        public DbSet<Kontobewegung> Kontobewegungen { get; set; }

        //Vererbung
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Konto>().ToTable("konten");

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Konto>()
                .HasDiscriminator<string>(k => k.Typ)
                .HasValue<Privatkonto>("Privatkonto")
                .HasValue<Sparkonto>("Sparkonto");

            // ---- Kontobewegung (Audit-Log) ----
            modelBuilder.Entity<Kontobewegung>().ToTable("kontobewegungen");

            // Enum als lesbaren String speichern ("Einzahlung" statt 0)
            modelBuilder.Entity<Kontobewegung>()
                .Property(b => b.Typ)
                .HasConversion<string>();

            // schnelle Abfrage: Bewegungen eines Kontos, neueste zuerst
            modelBuilder.Entity<Kontobewegung>()
                .HasIndex(b => new { b.Kontonummer, b.Zeitpunkt });

            // Beziehung zum Konto + Cascade: wird ein Konto geloescht,
            // verschwinden seine Bewegungen mit (Entscheidung: mitloeschen).
            modelBuilder.Entity<Kontobewegung>()
                .HasOne<Konto>()
                .WithMany()
                .HasForeignKey(b => b.Kontonummer)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
