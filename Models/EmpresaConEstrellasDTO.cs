using axcan.Models;

namespace axcan.Models
{
    public class EmpresaConEstrellasDTO
    {
        public Empresa Empresa { get; set; }
        public double PromedioEstrellas { get; set; }
        public int TotalResenas { get; set; }
    }
}