using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Taller_Industrial_Brenes_API.Models;

namespace Taller_Industrial_Brenes_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UsuarioController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("/ObtenerUsuarios")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                if (string.IsNullOrWhiteSpace(cs))
                    return StatusCode(500, new { mensaje = "Cadena de conexión 'ConexionBD' no configurada." });

                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("ObtenerTodosUsuarios", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await conn.OpenAsync();

                var lista = new List<UsuarioModel>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    lista.Add(new UsuarioModel
                    {
                        UsuarioID = reader.GetInt64(reader.GetOrdinal("UsuarioID")),
                        Identificacion = reader.GetString(reader.GetOrdinal("Identificacion")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        ApellidoPaterno = reader.GetString(reader.GetOrdinal("ApellidoPaterno")),
                        ApellidoMaterno = reader.GetString(reader.GetOrdinal("ApellidoMaterno")),
                        Correo = reader.GetString(reader.GetOrdinal("Correo")),
                        // Si no quieres exponer la contraseña en el GET:
                        Contrasenna = null,
                        TieneContrasennaTemp = reader.IsDBNull(reader.GetOrdinal("TieneContrasennaTemp"))
                                                 ? (bool?)null
                                                 : reader.GetBoolean(reader.GetOrdinal("TieneContrasennaTemp")),
                        FechaVencimientoTemp = reader.IsDBNull(reader.GetOrdinal("FechaVencimientoTemp"))
                                                 ? (DateTime?)null
                                                 : reader.GetDateTime(reader.GetOrdinal("FechaVencimientoTemp")),
                        Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                        RolID = reader.GetInt64(reader.GetOrdinal("RolID"))
                        // Si deseas el nombre del rol, añade una propiedad NombreRol a UsuarioModel
                        // NombreRol = reader.GetString(reader.GetOrdinal("NombreRol"))
                    });
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener todos los usuarios.", detalle = ex.Message });
            }
        }


        [HttpGet("/ObtenerPorID{id:long}")]
        public async Task<IActionResult> ObtenerPorID(long id)
        {
            if (id <= 0)
                return BadRequest("El ID de usuario debe ser mayor que cero.");

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                if (string.IsNullOrWhiteSpace(cs))
                    return StatusCode(500, new { mensaje = "Cadena de conexión 'ConexionBD' no configurada." });

                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("ObtenerUsuarioPorId", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UsuarioID", id);

                await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return NotFound(new { mensaje = $"No se encontró ningún usuario con ID = {id}" });

                var usuario = new UsuarioModel
                {
                    UsuarioID = reader.GetInt64(reader.GetOrdinal("UsuarioID")),
                    Identificacion = reader.GetString(reader.GetOrdinal("Identificacion")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    ApellidoPaterno = reader.GetString(reader.GetOrdinal("ApellidoPaterno")),
                    ApellidoMaterno = reader.GetString(reader.GetOrdinal("ApellidoMaterno")),
                    Correo = reader.GetString(reader.GetOrdinal("Correo")),
                    Contrasenna = null,
                    TieneContrasennaTemp = reader.IsDBNull(reader.GetOrdinal("TieneContrasennaTemp"))
                                             ? (bool?)null
                                             : reader.GetBoolean(reader.GetOrdinal("TieneContrasennaTemp")),
                    FechaVencimientoTemp = reader.IsDBNull(reader.GetOrdinal("FechaVencimientoTemp"))
                                             ? (DateTime?)null
                                             : reader.GetDateTime(reader.GetOrdinal("FechaVencimientoTemp")),
                    Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                    RolID = reader.GetInt64(reader.GetOrdinal("RolID"))
                };

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el usuario.", detalle = ex.Message });
            }
        }
        [HttpPut("ActualizarPerfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] UsuarioModel model)
        {
            // 1. Validar que venga un model válido
            if (model == null || model.UsuarioID <= 0)
                return BadRequest("Datos de usuario inválidos.");

            try
            {
                // 2. Leer la cadena de conexión "ConexionBD" desde appsettings.json
                var connectionString = _config.GetConnectionString("ConexionBD");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    // Si no se encontró la cadena, devolvemos un error claro
                    return StatusCode(500, new { mensaje = "No se encontró la cadena de conexión 'ConexionBD' en appsettings.json." });
                }

                // 3. Crear y configurar el comando para llamar al Stored Procedure
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("ActualizarPerfil", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // 4. Agregar todos los parámetros que el SP necesita
                    command.Parameters.AddWithValue("@UsuarioID", model.UsuarioID);
                    command.Parameters.AddWithValue("@Identificacion", model.Identificacion);
                    command.Parameters.AddWithValue("@Nombre", model.Nombre);
                    command.Parameters.AddWithValue("@ApellidoPaterno", model.ApellidoPaterno);
                    command.Parameters.AddWithValue("@ApellidoMaterno", model.ApellidoMaterno);
                    command.Parameters.AddWithValue("@Correo", model.Correo);
                    command.Parameters.AddWithValue("@Contrasenna", model.Contrasenna);

                    // Parámetros opcionales (pueden ser NULL)
                    if (model.TieneContrasennaTemp.HasValue)
                        command.Parameters.AddWithValue("@TieneContrasennaTemp", model.TieneContrasennaTemp.Value);
                    else
                        command.Parameters.AddWithValue("@TieneContrasennaTemp", DBNull.Value);

                    if (model.FechaVencimientoTemp.HasValue)
                        command.Parameters.AddWithValue("@FechaVencimientoTemp", model.FechaVencimientoTemp.Value);
                    else
                        command.Parameters.AddWithValue("@FechaVencimientoTemp", DBNull.Value);

                    command.Parameters.AddWithValue("@Estado", model.Estado);
                    command.Parameters.AddWithValue("@RolID", model.RolID);

                    // 5. Abrir la conexión y ejecutar el procedimiento
                    await connection.OpenAsync();
                    int filasAfectadas = await command.ExecuteNonQueryAsync();

                    // 6. Si no se actualizó ninguna fila, devolver 404 Not Found
                    if (filasAfectadas == 0)
                        return NotFound(new { mensaje = $"No se encontró ningún usuario con ID = {model.UsuarioID}" });

                    // 7. Si todo salió bien, devolvemos No Content (204)
                    return NoContent();
                }
            }
            catch (SqlException ex)
            {
                // 8. Si el SP lanzó RAISERROR (por correo duplicado), capturarlo aquí
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception exGeneral)
            {
                // 9. Cualquier otro error inesperado
                return StatusCode(500, new { mensaje = "Error interno al actualizar perfil.", detalle = exGeneral.Message });
            }
        }

    }
}
