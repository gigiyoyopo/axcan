using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("empresas", Schema = "public")]
   public class Empresa
{
    [Key]
    public int id_empresa { get; set; }
    public string nombre_empresa { get; set; }
    public int? id_administrador { get; set; }
    
    // Para los Numeric(10,8) de Supabase
    public decimal? ubicacion_lat { get; set; }
    public decimal? ubicacion_lng { get; set; }
    
    public string? rubro { get; set; }
    public string? logotipo_url { get; set; } // Opcional por ahora
}}