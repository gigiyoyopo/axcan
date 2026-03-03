using System.ComponentModel.DataAnnotations; 

namespace axcan.Models
{
    public class Cita
    {
        [Key] // Ahora sí va a reconocer esto
        public int id_cita { get; set; }
        
        public int id_expediente { get; set; }
        public int id_empresa { get; set; }
        public int id_usuario_tramito { get; set; }
        
        public int? id_servicio { get; set; }
        public int? id_prestador { get; set; }

        public DateTime fecha { get; set; }
        public TimeSpan hora { get; set; }
        
        public decimal? precio_final { get; set; }
        public string estatus { get; set; } = "pendiente";
        public string? notas_adicionales { get; set; }
    }
}