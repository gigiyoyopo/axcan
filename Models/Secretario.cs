using System.ComponentModel.DataAnnotations; // 👈 Asegúrate de agregar esta línea

public class Secretario
{
    [Key] // 👈 Esto le dice a EF: "Esta es la llave primaria"
    public int id_secretario { get; set; }
    
    public int? id_usuario { get; set; }
    public int? id_empresa { get; set; }
    public string? subrol { get; set; }
}