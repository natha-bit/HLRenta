using HLRenta.web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HLRenta.web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
    

     public DbSet<Vehiculo> Vehiculos { get; set; }
     public DbSet<Cliente> Clientes { get; set; }
     public DbSet<Reserva> Reservas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para almacenar la lista de imágenes como JSON (si usas EF Core 5+)
            modelBuilder.Entity<Vehiculo>()
                .Property(v => v.Imagenes)
                .HasConversion(
                    v => string.Join(";", v),             // convertir List<string> a string
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            modelBuilder.Entity<Cliente>()
               .HasIndex(c => c.NumeroLicencia)
               .IsUnique();
        }
    }
 }
