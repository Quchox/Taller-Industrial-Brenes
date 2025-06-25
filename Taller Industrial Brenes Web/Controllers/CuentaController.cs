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
        public async Task<IActionResult> Login(UsuarioModel model)
        {

            // Encriptación 
            model.Contrasenna = Encrypt(model.Contrasenna!);

            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync($"{_apiUrl}/Autenticacion/Login", model);

            if (response.IsSuccessStatusCode)
            {
                var usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();
                if (usuario != null)
                {
                    // Guardar datos en la sesión
                    HttpContext.Session.SetString("Token", usuario!.Token!);
                    HttpContext.Session.SetString("Correo", usuario!.Correo!);
                    HttpContext.Session.SetString("RolID", usuario!.RolID.ToString());
                    HttpContext.Session.SetString("UsuarioID", usuario!.UsuarioID.ToString());
                    HttpContext.Session.SetString("Nombre", usuario!.Nombre.ToString());

                    // Redirigir según el Rol
                    if (usuario.RolID == 1)
                    {
                        return RedirectToAction("IndexAdmin","Admin");
                    }
                    else if (usuario.RolID == 2)
                    {
                        return RedirectToAction("Index","Home");
                    }
                }

                ViewBag.Error = "No se pudo procesar la información del usuario.";
                return View();
            }

            ViewBag.Error = "Correo o contraseña inválidos";
            return View();
        }

        [HttpGet]
        public IActionResult Registro() => View();

        [HttpPost]
        public async Task<IActionResult> Registro(UsuarioModel model)
        {
            // Encriptación 
            model.Contrasenna = Encrypt(model.Contrasenna!);
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync($"{_apiUrl}/Autenticacion/Registro", model);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            ViewBag.Error = "Error al registrar el usuario";
            return View();
        }

        #region Cifrar/Descrifrar Contraseñas
        private string Encrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_config.GetSection("llaveCifrado").Value!);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(texto);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public string Decrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(texto);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_config.GetSection("llaveCifrado").Value!);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        #endregion

    }
}
