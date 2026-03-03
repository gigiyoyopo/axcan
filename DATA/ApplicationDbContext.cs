using Microsoft.EntityFrameworkCore;
using axcan.Models; // <-- ESTO ES LO QUE LE FALTA PARA ENCONTRAR 'SERVICIO'

namespace axcan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- TABLAS DEL SISTEMA ---
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Empresa> empresas { get; set; }
        public DbSet<Servicio> servicios { get; set; }
        public DbSet<Secretario> secretarios { get; set; } 
        public DbSet<HorarioNegocio> horarios_negocio { get; set; }
        public DbSet<Cita> citas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Forzamos a que usen el esquema public de Supabase
            modelBuilder.Entity<Usuario>().ToTable("usuarios", schema: "public");
            modelBuilder.Entity<Empresa>().ToTable("empresas", schema: "public");
            modelBuilder.Entity<Servicio>().ToTable("servicios", schema: "public");
            modelBuilder.Entity<Secretario>().ToTable("secretarios", schema: "public");
            modelBuilder.Entity<HorarioNegocio>().ToTable("horarios_negocio", schema: "public");
            modelBuilder.Entity<Cita>().ToTable("citas", schema: "public");
        }
    }
}