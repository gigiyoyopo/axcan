using Microsoft.EntityFrameworkCore;
using axcan.Models; // <--- ESTO ARREGLA EL ERROR CS0246

namespace axcan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tus tablas
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Empresa> empresas { get; set; } // <--- Aquí usa Empresa

        // SOLO UN MÉTODO OnModelCreating (ESTO ARREGLA EL ERROR CS0111)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuramos la tabla de usuarios
            modelBuilder.Entity<Usuario>().ToTable("usuarios", schema: "public");

            // Configuramos la tabla de empresas
            modelBuilder.Entity<Empresa>().ToTable("empresas", schema: "public");
        }
    }
}