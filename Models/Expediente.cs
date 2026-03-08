using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
 [Table("expedientes", Schema = "public")]
public class Expediente
{
    [Key]
    public int id_expediente { get; set; }
    
    // Debe coincidir con id_usuario de la tabla clientes
    public int id_cliente { get; set; } 
    
    // Campo obligatorio y único en tu esquema
    public string folio { get; set; } 
    
    public string estatus { get; set; } = "PENDIENTE";
    public string? descripcion { get; set; }
    
    // Nombre exacto en tu base de datos
    [Column("fecha_creacion")]
    public DateTime? fecha_creacion { get; set; } = DateTime.Now;
}  
}