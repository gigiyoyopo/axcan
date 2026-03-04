public class Empresa
{
    [Key]
    public int id_empresa { get; set; }
    public int id_administrador { get; set; }
    public string nombre_empresa { get; set; }
    public string? logotipo_url { get; set; }
    public string? banner_url { get; set; } 
    public string? color_tema { get; set; }
    public double ubicacion_lat { get; set; }
    public double ubicacion_lng { get; set; }
    public string? descripcion { get; set; } 
    public int id_plantilla { get; set; } 
    public string rubro { get; set; }
}