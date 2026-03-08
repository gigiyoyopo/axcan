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
        
        
        public string rol { get; set; } = "cliente"; 
public string? foto_url { get; set; } 
        public DateTime? fecha_registro { get; set; } = DateTime.Now;
    }
}