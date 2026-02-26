using Microsoft.EntityFrameworkCore;
using axcan.Models;

namespace axcan.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Usuario>()
        .ToTable("usuarios", schema: "public"); // Forzamos el nombre en minúsculas

    // Si te da lata el ENUM de 'tipo_rol', esto lo arregla:
    modelBuilder.HasPostgresEnum<tipo_rol>(); 
}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> usuarios { get; set; } // Nombre en minúscula igual que en Supabase
    }
    
}