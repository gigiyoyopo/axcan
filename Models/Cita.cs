using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("citas", Schema = "public")]
    public class Cita
    {
        [Key]
        public int id_cita { get; set; }
        public int id_expediente { get; set; }
        public int id_empresa { get; set; }
        public int id_usuario_tramito { get; set; }
        
        // Aquí estaba el error: se llama "fecha", no "fecha_cita"
        public DateTime fecha { get; set; } 
        
        // Se llama "hora", no "hora_cita" y en Postgres es TIME (TimeSpan en C#)
        public TimeSpan hora { get; set; } 
        
        public string? tipo_servicio { get; set; }
        public decimal? precio { get; set; }
        public string? quien_atiende { get; set; }
        public string? estatus { get; set; } = "pendiente";
    }
}