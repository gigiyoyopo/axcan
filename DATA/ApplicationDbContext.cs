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
        public DbSet<Cliente> clientes { get; set; } 
        public DbSet<Expediente> expedientes { get; set; }
        public DbSet<Empresa> empresas { get; set; }
        public DbSet<Servicio> servicios { get; set; } 
        public DbSet<Secretario> secretarios { get; set; }
        public DbSet<Cita> citas { get; set; }
        public DbSet<HorarioNegocio> horarios_negocio { get; set; }
        public DbSet<Resena> resenas { get; set; }
public DbSet<GaleriaFoto> galeria_fotos { get; set; } // Mapeo de la tabla de Supabase
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Forzamos el mapeo a las tablas de Supabase en el esquema public
            modelBuilder.Entity<Usuario>().ToTable("usuarios", schema: "public");
            modelBuilder.Entity<Cliente>().ToTable("clientes", schema: "public");
            modelBuilder.Entity<Expediente>().ToTable("expedientes", schema: "public");
            modelBuilder.Entity<Cita>().ToTable("citas", schema: "public");
            modelBuilder.Entity<Empresa>().ToTable("empresas", schema: "public");
            modelBuilder.Entity<Servicio>().ToTable("servicios", schema: "public");
            modelBuilder.Entity<HorarioNegocio>().ToTable("horarios_negocio", schema: "public");
            modelBuilder.Entity<Secretario>().ToTable("secretarios", schema: "public");
            modelBuilder.Entity<Resena>().ToTable("resenas", schema: "public");
        }
    }
}