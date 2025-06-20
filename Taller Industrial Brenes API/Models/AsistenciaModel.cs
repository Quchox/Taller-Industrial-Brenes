
using System;

namespace Taller_Industrial_Brenes_API.Models
{
    public class AsistenciaModel
    {
        public long AsistenciaID { get; set; }
        public long UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public string Estado { get; set; }
    }
}
