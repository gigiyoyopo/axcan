using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("usuarios", Schema = "public")]
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; } // En tu SQL es id_usuario, no id
        
        public string username { get; set; }
        public string password { get; set; }
        public string correo { get; set; }
        public string nombre { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        
        [Column(TypeName = "tipo_rol")] // Tu ENUM personalizado
        public string rol { get; set; } = "cliente"; 
        
        public DateTime fecha_registro { get; set; } = DateTime.Now;
    }
}