using System.ComponentModel.DataAnnotations;

namespace axcan.Models
{
    public class Empresa
    {
        [Key]
        public int id_empresa { get; set; }

        public string? nombre_empresa { get; set; }
        public string? rubro { get; set; }
        public string? logotipo_url { get; set; }
        
        // --- ESTAS SON LAS QUE FALTABAN ---
        public string? color_tema { get; set; } = "#253745"; // Color por defecto
        public string? banner_url { get; set; }
        public int id_plantilla { get; set; } = 1; 

        public double ubicacion_lat { get; set; }
        public double ubicacion_lng { get; set; }
        
        public int id_administrador { get; set; }
    }
}