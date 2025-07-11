using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Taller_Industrial_Brenes_API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Taller_Industrial_Brenes_API.Controllers.Admin
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IConfiguration _config;
        public UsuariosController(IConfiguration config)
        {
            _config = config;

        }

        [HttpGet("listado")]
        public IActionResult ListadoAdmin([FromQuery] long UsuarioID)
        {
            using (var context = new SqlConnection(_config.GetConnectionString("ConexionBD")))
            {
                var result = context.Query<UsuarioModel>("ConsultarUsuario", new { UsuarioID });

                if (result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound("No hay clientes registrados en este momento");
                }
            }
        }

        [HttpPut]
        [Route("DesactivarUsuario")]
        public IActionResult DesactivarUsuario([FromBody] long usuarioID)
        {
            using (var connection = new SqlConnection(_config.GetSection("ConnectionStrings:ConexionBD").Value))
            {
                var result = connection.Execute("DesactivarUsuario", new { UsuarioID = usuarioID });

                if (result > 0)
                {
                    return Ok("Usuario desactivado correctamente.");
                }
                else
                {
                    return BadRequest("No se pudo desactivar el usuario.");
                }
            }
        }

        [HttpPut]
        [Route("ActivarUsuario")]
        public IActionResult ActivarUsuario([FromBody] long usuarioID)
        {
            using (var connection = new SqlConnection(_config.GetSection("ConnectionStrings:ConexionBD").Value))
            {
                var result = connection.Execute("ActivarUsuario", new { UsuarioID = usuarioID });

                if (result > 0)
                {
                    return Ok("Usuario activado correctamente.");
                }
                else
                {
                    return BadRequest("No se pudo activar el usuario.");
                }
            }
        }

        [HttpPut]
        [Route("ActualizarRol")]
        public IActionResult ActualizarRol([FromBody] RolModel model)
        {
            using (var connection = new SqlConnection(_config.GetSection("ConnectionStrings:ConexionBD").Value))
            {
                var result = connection.QuerySingle<int>("CambiarRolUsuario", new
                {
                    UsuarioID = model.UsuarioID
                }, commandType: CommandType.StoredProcedure);

                if (result == 1)
                    return Ok("Rol actualizado correctamente.");
                else if (result == 0)
                    return BadRequest("El rol actual no permite cambio.");
                else // -1
                    return BadRequest("Usuario no encontrado.");
            }
        }

    }
}