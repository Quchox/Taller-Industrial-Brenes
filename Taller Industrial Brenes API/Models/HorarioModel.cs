using System.ComponentModel.DataAnnotations;

namespace Taller_Industrial_Brenes_API.Models
{
    public class HorarioModel
    {
        public int HorarioID { get; set; }

        [Required(ErrorMessage = "El UsuarioID es obligatorio.")]
        [Range(1, long.MaxValue, ErrorMessage = "UsuarioID debe ser mayor que cero.")]
        public long UsuarioID { get; set; }

        [Required(ErrorMessage = "El día de la semana es obligatorio.")]
        [RegularExpression("^(Lunes|Martes|Miércoles|Miercoles|Jueves|Viernes|Sábado|Sabado|Domingo)$", ErrorMessage = "Día de la semana inválido.")]
        public string DiaSemana { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        public TimeSpan HoraFin { get; set; }

        public string Observaciones { get; set; }
    }
}