
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Taller_Industrial_Brenes_API.Models;

namespace Taller_Industrial_Brenes_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciaController : ControllerBase
    {
        private readonly string _connectionString;

        public AsistenciaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionBD");
        }

        [HttpPost("marcar-entrada")]
        public async Task<IActionResult> MarcarEntrada([FromBody] AsistenciaModel model)
        {
            if (model.UsuarioID <= 0 || model.HoraEntrada == null)
                return BadRequest("Datos inválidos.");

            using SqlConnection conexion = new SqlConnection(_connectionString);
            using SqlCommand comando = new SqlCommand("MarcarEntrada", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@UsuarioID", model.UsuarioID);
            comando.Parameters.AddWithValue("@HoraEntrada", model.HoraEntrada);

            try
            {
                await conexion.OpenAsync();
                await comando.ExecuteNonQueryAsync();
                return Ok("Entrada registrada correctamente.");
            }
            catch (SqlException ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("marcar-salida")]
        public async Task<IActionResult> MarcarSalida([FromBody] AsistenciaModel model)
        {
            if (model.UsuarioID <= 0 || model.HoraSalida == null)
                return BadRequest("Datos inválidos.");

            using SqlConnection conexion = new SqlConnection(_connectionString);
            using SqlCommand comando = new SqlCommand("MarcarSalida", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@UsuarioID", model.UsuarioID);
            comando.Parameters.AddWithValue("@HoraSalida", model.HoraSalida);

            try
            {
                await conexion.OpenAsync();
                await comando.ExecuteNonQueryAsync();
                return Ok("Salida registrada correctamente.");
            }
            catch (SqlException ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("usuario/{id}")]
        public async Task<ActionResult<List<AsistenciaModel>>> ObtenerPorUsuario(long id)
        {
            var lista = new List<AsistenciaModel>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand comando = new SqlCommand("ListarAsistenciaPorUsuario", conn);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@UsuarioID", id);

            try
            {
                await conn.OpenAsync();
                using var reader = await comando.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    lista.Add(new AsistenciaModel
                    {
                        AsistenciaID = reader.GetInt64(0),
                        UsuarioID = reader.GetInt64(1),
                        Fecha = reader.GetDateTime(2),
                        HoraEntrada = reader.IsDBNull(3) ? null : reader.GetTimeSpan(3),
                        HoraSalida = reader.IsDBNull(4) ? null : reader.GetTimeSpan(4),
                        Estado = reader.IsDBNull(5) ? null : reader.GetString(5)
                    });
                }

                return Ok(lista);
            }
            catch (SqlException ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
