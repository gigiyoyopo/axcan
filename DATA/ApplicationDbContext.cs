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

        // Definición de las tablas en C#
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Empresa> empresas { get; set; }
        public DbSet<Servicio> servicios { get; set; }
        public DbSet<Secretario> secretarios { get; set; }
        public DbSet<HorarioNegocio> horarios_negocio { get; set; }
        public DbSet<Cita> citas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Esto asegura que Render y Supabase se entiendan con los nombres de las tablas
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Empresa>().ToTable("empresas");
            modelBuilder.Entity<Servicio>().ToTable("servicios");
            modelBuilder.Entity<Secretario>().ToTable("secretarios");
            modelBuilder.Entity<HorarioNegocio>().ToTable("horarios_negocio");
            modelBuilder.Entity<Cita>().ToTable("citas");
        }
    }
}