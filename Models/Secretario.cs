using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("secretarios", Schema = "public")]
    public class Secretario
    {
        [Key]
        public int id_secretario { get; set; }
        public int id_usuario { get; set; }
        public int id_empresa { get; set; } 
        public string subrol { get; set; }
    }
}