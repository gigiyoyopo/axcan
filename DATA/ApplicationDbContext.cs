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

        // Esta es tu tabla en la base de datos
        public DbSet<Usuario> usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Forzamos a que EF busque la tabla 'usuarios' en el esquema 'public'
            modelBuilder.Entity<Usuario>()
                .ToTable("usuarios", schema: "public");

            // Nota: No incluimos el mapeo del ENUM 'tipo_rol' aquí 
            // para evitar errores de compilación si no existe la clase en C#.
            // El campo 'rol' en el modelo Usuario se tratará como un string común.
        }
    }
}