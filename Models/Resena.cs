// Guarda las calificaciones y comentarios que los clientes le dejan a las barberías/clínicas.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("resenas", Schema = "public")]
    public class Resena
    {
        [Key]
        public int id_resena { get; set; }
        public int id_empresa { get; set; }
        public int id_cliente { get; set; } // El usuario que dejó la reseña
        public int calificacion { get; set; } // Del 1 al 5
        public string? comentario { get; set; }
        public DateTime fecha_resena { get; set; } = DateTime.Now;
    }
}