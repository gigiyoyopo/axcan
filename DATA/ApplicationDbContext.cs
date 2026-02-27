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
        }
    public DbSet<Empresa> empresas { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<Empresa>().ToTable("empresas", schema: "public");
}
    }
}