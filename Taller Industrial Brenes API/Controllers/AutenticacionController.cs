using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;
using Taller_Industrial_Brenes_API.Models;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Taller_Industrial_Brenes_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutenticacionController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AutenticacionController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequestModel login)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("ConexionBD"));

            var usuario = connection.QueryFirstOrDefault<UsuarioModel>(
                "LoginUsuario",
                new { login.Correo, login.Contrasenna },
                commandType: System.Data.CommandType.StoredProcedure);

            if (usuario == null)
                return Unauthorized("Credenciales incorrectas");

            return Ok(usuario);
        }


        [HttpPost("Registro")]
        public IActionResult Registro([FromBody] UsuarioModel nuevo)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("ConexionBD"));

            try
            {
                connection.Execute(
                    "RegistrarUsuario",
                    new
                    {
                        nuevo.Identificacion,
                        nuevo.Nombre,
                        nuevo.ApellidoPaterno,
                        nuevo.ApellidoMaterno,
                        nuevo.Correo,
                        nuevo.Contrasenna,
                        nuevo.RolID
                    },
                    commandType: System.Data.CommandType.StoredProcedure);

                return Ok("Usuario registrado exitosamente");
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EnviarRecuperacion")]
        public IActionResult EnviarRecuperacion([FromBody] string correo)
        {
            try
            {
                using (var context = new SqlConnection(_config.GetConnectionString("ConexionBD")))
                {
                    //Procedimiento almacenado
                    var usuario = context.QueryFirstOrDefault<UsuarioModel>(
                        "ObtenerUsuarioPorCorreo",
                        new { correo },
                        commandType: System.Data.CommandType.StoredProcedure);

                    if (usuario == null)
                        return NotFound(new { mensaje = "El correo no está registrado.", correo });

                    var codigo = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

                    var asunto = "Recuperación de Contraseña - Taller Industrial Brenes";
                    var cuerpo = $"Hola {usuario.Nombre},\n\nTu código de recuperación es: {codigo}\n\n";

                    var enviado = UtilidadesCorreo.EnviarCorreo(correo, asunto, cuerpo);

                    if (enviado)
                    {
                        return Ok(new { mensaje = "Correo enviado correctamente", codigo });
                    }
                    else
                    {
                        return StatusCode(500, new { mensaje = "No se pudo enviar el correo desde el servidor SMTP" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error interno",
                    error = ex.Message,
                    stack = ex.StackTrace
                });
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




