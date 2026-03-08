using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("clientes", Schema = "public")]
    public class Cliente
    {
        [Key]
        public int id_usuario { get; set; } // PK vinculada a usuarios 

        [Required]
        public string numero_telefono { get; set; } // Validación de 10 dígitos 
    }
}