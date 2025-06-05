using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;
using Taller_Industrial_Brenes_API.Models;

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
    }
}

