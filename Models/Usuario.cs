using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("usuarios", Schema = "public")]
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; }
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string apellido_paterno { get; set; } = string.Empty;
        public string apellido_materno { get; set; } = string.Empty;
        
        // Ahora es string para coincidir con el TEXT de SQL
        public string rol { get; set; } = "cliente"; 

        public DateTime? fecha_registro { get; set; } = DateTime.Now;
    }
}