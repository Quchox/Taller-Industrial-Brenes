using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;
using Taller_Industrial_Brenes_API.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Taller_Industrial_Brenes_API.Controllers
{
    [AllowAnonymous]
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
                using (var context = new SqlConnection(_config.GetConnectionString("ConnectionStrings:ConexionBD")))
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

        [HttpPut("RestablecerContrasenna")]
        public IActionResult RestablecerContrasenna([FromBody] UsuarioModel model)
        {
            using (var context = new SqlConnection(_config.GetConnectionString("ConnectionStrings:ConexionBD")))
            {
                // Ejecutar el procedimiento almacenado
                var result = context.Execute("RestablecerContrasenna",
                    new { model.Correo, model.Contrasenna },
                    commandType: System.Data.CommandType.StoredProcedure);

                if (result > 0)
                    return Ok(new { mensaje = "Contraseña actualizada correctamente" });

                return BadRequest(new { mensaje = "No se pudo actualizar la contraseña" });
            }
        }

        private string GenerarToken(long UsuarioID, long RolID)
        {
            string? SecretKey = _config.GetSection("Variables:llaveCifrado").Value!;

            if (string.IsNullOrEmpty(SecretKey))
            {
                throw new InvalidOperationException("La llave del token no está configurada correctamente.");
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("UsuarioID", UsuarioID.ToString()));
            claims.Add(new Claim("RolID", RolID.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet("ObtenerUsuarioPorCorreo/{correo}")]
        public IActionResult ObtenerUsuarioPorCorreo(string correo)
        {
            using (var context = new SqlConnection(_config.GetSection("ConnectionStrings:ConexionBD").Value))
            {
                var result = context.QueryFirstOrDefault<UsuarioModel>("ObtenerUsuarioCompleto", new { correo }, commandType: System.Data.CommandType.StoredProcedure);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
        }
    }
}




