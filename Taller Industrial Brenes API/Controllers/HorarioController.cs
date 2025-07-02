using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Taller_Industrial_Brenes_API.Models;
using System.Data;
using System.Collections.Generic;

namespace Taller_Industrial_Brenes_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HorarioController : ControllerBase
    {
        private IConfiguration _config;

        public HorarioController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] HorarioModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.HoraInicio >= model.HoraFin)
                return BadRequest(new { mensaje = "La hora de inicio debe ser menor que la hora de fin." });

            using var conn = new SqlConnection(_config.GetConnectionString("ConexionBD"));
            using var comando = new SqlCommand("CrearHorario", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            comando.Parameters.AddWithValue("@UsuarioID", model.UsuarioID);
            comando.Parameters.AddWithValue("@DiaSemana", model.DiaSemana);
            comando.Parameters.AddWithValue("@HoraInicio", model.HoraInicio);
            comando.Parameters.AddWithValue("@HoraFin", model.HoraFin);
            comando.Parameters.AddWithValue("@Observaciones", model.Observaciones ?? (object)DBNull.Value);

            try
            {
                await conn.OpenAsync();
                await comando.ExecuteNonQueryAsync();
                return Ok("Horario creado exitosamente.");
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("PorUsuario/{usuarioId:long}")]
        public async Task<IActionResult> ObtenerPorUsuario(long usuarioId)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("ConexionBD"));
            using var comando = new SqlCommand("ObtenerHorariosPorUsuario", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            comando.Parameters.AddWithValue("@UsuarioID", usuarioId);

            var lista = new List<HorarioModel>();
            await conn.OpenAsync();
            using var reader = await comando.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new HorarioModel
                {
                    HorarioID = reader.GetInt32(reader.GetOrdinal("HorarioID")),
                    UsuarioID = usuarioId,
                    DiaSemana = reader.GetString(reader.GetOrdinal("DiaSemana")),
                    HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                    HoraFin = reader.GetTimeSpan(reader.GetOrdinal("HoraFin")),
                    Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones"))
                });
            }
            return Ok(lista);
        }

        [HttpPut("Actualizar")]
        public async Task<IActionResult> Actualizar([FromBody] HorarioModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.HorarioID <= 0)
                return BadRequest(new { mensaje = "Debe especificar un HorarioID válido para actualizar." });

            if (model.HoraInicio >= model.HoraFin)
                return BadRequest(new { mensaje = "La hora de inicio debe ser menor que la hora de fin." });

            using var conn = new SqlConnection(_config.GetConnectionString("ConexionBD"));
            using var comando = new SqlCommand("ActualizarHorario", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            comando.Parameters.AddWithValue("@HorarioID", model.HorarioID);
            comando.Parameters.AddWithValue("@DiaSemana", model.DiaSemana);
            comando.Parameters.AddWithValue("@HoraInicio", model.HoraInicio);
            comando.Parameters.AddWithValue("@HoraFin", model.HoraFin);
            comando.Parameters.AddWithValue("@Observaciones", model.Observaciones ?? (object)DBNull.Value);

            try
            {
                await conn.OpenAsync();
                await comando.ExecuteNonQueryAsync();
                return Ok("Horario actualizado exitosamente.");
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("Eliminar/{horarioId:int}")]
        public async Task<IActionResult> Eliminar(int horarioId)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("ConexionBD"));
            using var comando = new SqlCommand("EliminarHorario", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            comando.Parameters.AddWithValue("@HorarioID", horarioId);

            await conn.OpenAsync();
            await comando.ExecuteNonQueryAsync();
            return Ok("Horario eliminado exitosamente.");
        }
    }
}