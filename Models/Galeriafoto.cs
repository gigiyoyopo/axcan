using System.ComponentModel.DataAnnotations;

namespace axcan.Models // <--- Este debe coincidir con el 'using' del paso anterior
{
    public class GaleriaFoto
    {
        [Key]
        public int id_foto { get; set; }
        public int id_empresa { get; set; }
        public string url { get; set; }
 
    }
}