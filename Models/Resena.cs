using System.ComponentModel.DataAnnotations; // <--- No olvides este using

namespace axcan.Models
{
    public class Resena
    {
        [Key] // <--- ESTO MATARÁ EL ERROR ROJO
        public int id_resena { get; set; }
        public int id_empresa { get; set; }
        public int id_usuario { get; set; }
        public string comentario { get; set; } = string.Empty; // El = string.Empty quita el warning CS8618
        public int calificacion { get; set; }
        public DateTime fecha { get; set; }
    }
}