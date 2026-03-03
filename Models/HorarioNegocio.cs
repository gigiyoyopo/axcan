using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axcan.Models
{
    [Table("horarios_negocio", Schema = "public")]
    public class HorarioNegocio
    {
        [Key]
        public int id_horario { get; set; }
        
        public int id_empresa { get; set; }
        
        public int dia_semana { get; set; } // 0-6
        
        public TimeSpan hora_apertura { get; set; }
        
        public TimeSpan hora_cierre { get; set; }
        
        public bool es_dia_descanso { get; set; } = false;
    }
}