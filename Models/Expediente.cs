using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("expedientes", Schema = "public")]
    public class Expediente
    {
        [Key]
        public int id_expediente { get; set; }
        
        public int id_cliente { get; set; } // Coincide con tu FK id_cliente
        
        public string folio { get; set; } // Obligatorio y único
        
        public string? estatus { get; set; } = "PENDIENTE";
        
        public string? descripcion { get; set; }
        
        public DateTime? fecha_creacion { get; set; } = DateTime.Now;
    }
}