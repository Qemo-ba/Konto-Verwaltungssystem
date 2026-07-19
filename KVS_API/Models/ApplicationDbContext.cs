using Microsoft.EntityFrameworkCore;

namespace KVS_API.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Konto> Konten {  get; set; }

        //Vererbung
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Konto>().ToTable("konten");

            // Sicherheitsnetz auf DB-Ebene: keine doppelten E-Mails / Usernamen,
            // selbst wenn zwei Registrierungen die App-Pruefung gleichzeitig passieren.
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Konto>()
                .HasDiscriminator<string>(k => k.Typ)
                .HasValue<Privatkonto>("Privatkonto")
                .HasValue<Sparkonto>("Sparkonto");
        }


    }
}
