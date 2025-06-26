using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Taller_Industrial_Brenes_Web.Models;

namespace Taller_Industrial_Brenes_Web.Controllers
{
	public class CuentaController : Controller
	{
		private readonly IConfiguration _config;
		private readonly string _apiUrl;
		private readonly string _llaveCifrado;

		public CuentaController(IConfiguration config)
		{
			_config = config;
			_apiUrl = _config["ApiUrl"];
			_llaveCifrado = _config["llaveCifrado"];
		}

		[HttpGet]
		public IActionResult Login() => View();

		[HttpPost]
		public async Task<IActionResult> Login(LoginRequestModel model)
		{
			// Encriptar contraseña antes de enviarla al API
			model.Contrasenna = Encrypt(model.Contrasenna ?? string.Empty);

			using var client = new HttpClient();
			var response = await client.PostAsJsonAsync($"{_apiUrl}/Autenticacion/Login", model);

			if (response.IsSuccessStatusCode)
			{
				var usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();

				if (usuario != null)
				{
					TempData["UsuarioNombre"] = usuario.Nombre;
					TempData["RolID"] = usuario.RolID.ToString();
					TempData.Keep("RolID");

					// Redirección según el RolID
					if (usuario.RolID == 2)
					{
						return RedirectToAction("Index", "Home");
					}
					else if (usuario.RolID == 1)
					{
						return RedirectToAction("ListadoAdmin", "Clientes");
					}
					else
					{
						// Si no es ninguno de los roles esperados
						return RedirectToAction("Login");
					}
				}
			}

			ViewBag.Error = "Correo o contraseña inválidos";
			return View();
		}

		[HttpGet]
		public IActionResult Registro() => View();

		[HttpPost]
		public async Task<IActionResult> Registro(UsuarioModel model)
		{
			// Encriptar contraseña antes de enviarla al API
			model.Contrasenna = Encrypt(model.Contrasenna ?? string.Empty);

			using var client = new HttpClient();
			var response = await client.PostAsJsonAsync($"{_apiUrl}/Autenticacion/Registro", model);

			if (response.IsSuccessStatusCode)
				return RedirectToAction("Login");

			ViewBag.Error = "Error al registrar el usuario";
			return View();
		}

		#region Encriptación AES
		private string Encrypt(string texto)
		{
			byte[] iv = new byte[16];
			byte[] array;

			using (Aes aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(_llaveCifrado);
				aes.IV = iv;

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using var memoryStream = new MemoryStream();
				using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
				using var streamWriter = new StreamWriter(cryptoStream);

				streamWriter.Write(texto);
				streamWriter.Flush();
				cryptoStream.FlushFinalBlock();

				array = memoryStream.ToArray();
			}

			return Convert.ToBase64String(array);
		}
		#endregion
	}
}