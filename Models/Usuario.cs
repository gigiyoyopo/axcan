using System.ComponentModel.DataAnnotations;

namespace axcan.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string NombreCompleto { get; set; } = string.Empty;
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string UserName { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty; // En el futuro le metemos Hash, por ahora así para que jale
    }
}