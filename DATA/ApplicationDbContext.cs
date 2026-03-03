using Microsoft.EntityFrameworkCore;
using axcan.Models;

namespace axcan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Empresa> empresas { get; set; }
        public DbSet<Servicio> servicios { get; set; } 
        public DbSet<Secretario> secretarios { get; set; }
        public DbSet<HorarioNegocio> horarios_negocio { get; set; }
        public DbSet<Cita> citas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Forzamos el mapeo a las tablas de Supabase
            modelBuilder.Entity<Servicio>().ToTable("servicios", schema: "public");
        }
    }
}