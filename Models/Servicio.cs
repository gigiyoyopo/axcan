using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("servicios", Schema = "public")]
    public class Servicio 
    {
        [Key]
        public int id_servicio { get; set; }
        public int id_empresa { get; set; }
        public string? nombre_servicio { get; set; }
        public string? descripcion { get; set; }
        public decimal precio { get; set; }
        public int duracion_minutos { get; set; }
        public bool activo { get; set; }
    }
}