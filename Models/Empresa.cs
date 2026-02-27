using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("empresas", Schema = "public")]
    public class Empresa
    {
        [Key]
        public int id_empresa { get; set; }

        public string nombre_empresa { get; set; } = string.Empty;

        // IDs de los usuarios (Admin y Secretario)
        public int? id_administrador { get; set; }
        public int? id_secretario { get; set; }

        // Coordenadas con la precisión de tu SQL
        [Column("ubicacion_lat")]
        public decimal? ubicacion_lat { get; set; }

        [Column("ubicacion_lng")]
        public decimal? ubicacion_lng { get; set; }

        public string? rubro { get; set; }
    }
}