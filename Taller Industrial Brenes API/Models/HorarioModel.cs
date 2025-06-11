namespace Taller_Industrial_Brenes_API.Models
{
    public class HorarioModel
    {
        public int HorarioID { get; set; }
        public long UsuarioID { get; set; }
        public string DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Observaciones { get; set; }
    }
}
