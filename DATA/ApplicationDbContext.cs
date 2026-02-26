using Microsoft.EntityFrameworkCore;
using axcan.Models;

namespace axcan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> usuarios { get; set; } // Nombre en minúscula igual que en Supabase
    }
}