using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("citas", Schema = "public")]
    public class Cita
    {
        [Key]
        public int id_cita { get; set; }
        public int id_empresa { get; set; }
        public int id_servicio { get; set; }
        public int? id_secretario { get; set; } 
        
      
        public DateTime fecha_cita { get; set; } 
        public string hora_cita { get; set; } 
        public string? notas { get; set; } 
        public string estatus { get; set; } = "pendiente";
        public DateTime fecha_creacion { get; set; } = DateTime.Now;
    }
}