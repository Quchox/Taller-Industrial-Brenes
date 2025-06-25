using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Taller_Industrial_Brenes_Web.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Taller_Industrial_Brenes_Web.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    public class AutenticacionController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClient;
        private readonly string _apiUrl;
        private readonly string _llaveCifrado;
        public AutenticacionController(IConfiguration config, IHttpClientFactory httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _apiUrl = _config["ApiUrl"];
            _llaveCifrado = _config["llaveCifrado"];

        }
        [HttpGet]
        public IActionResult RecuperarContrasenna()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarContrasenna(UsuarioModel model)
        {
            try
            {
                var cliente = _httpClient.CreateClient();
                var url = _apiUrl + "Autenticacion/EnviarRecuperacion";

                var content = new StringContent(JsonSerializer.Serialize(model.Correo), Encoding.UTF8, "application/json");

                var response = await cliente.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonDocument.Parse(json);
                    var codigo = data.RootElement.GetProperty("codigo").GetString();

                    HttpContext.Session.SetString("CorreoRecuperacion", model.Correo!);
                    HttpContext.Session.SetString("CodigoRecuperacion", codigo!);

                    TempData["Msj"] = "Código enviado a tu correo.";
                    return RedirectToAction("CodigoRecuperacion");
                }
                ViewBag.Msj = $"Error en API: {response.StatusCode}";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Msj = $"Error inesperado: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public IActionResult RestablecerContrasenna()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerContrasenna(UsuarioModel model, string ConfirmarContrasenna)
        {
            if (model.Contrasenna != ConfirmarContrasenna)
            {
                ViewBag.Msj = "Las contraseñas no coinciden.";
                return View();
            }

            var correo = HttpContext.Session.GetString("CorreoRecuperacion");

            if (string.IsNullOrEmpty(correo))
            {
                ViewBag.Msj = "No se encontró el correo. Vuelva a iniciar el proceso.";
                return RedirectToAction("RecuperarContrasenna");
            }

            model.Correo = correo;
            model.Contrasenna = Encrypt(model.Contrasenna!);

            var cliente = _httpClient.CreateClient();
            var url = _config["ApiUrl" +
                ""] + "Autenticacion/RestablecerContrasenna";

            var response = await cliente.PutAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Msj"] = "Contraseña actualizada correctamente.";
                HttpContext.Session.Remove("CorreoRecuperacion");
                HttpContext.Session.Remove("CodigoRecuperacion");
                return RedirectToAction("Login");
            }

            ViewBag.Msj = "No se pudo actualizar la contraseña.";
            return View();
        }

        [HttpGet]
        public IActionResult CodigoRecuperacion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CodigoRecuperacion(string correo, string codigo)
        {
            var correoEsperado = HttpContext.Session.GetString("CorreoRecuperacion");
            var codigoEsperado = HttpContext.Session.GetString("CodigoRecuperacion");

            if (correo != correoEsperado)
            {
                ViewBag.Msj = "El correo no coincide con el solicitado.";
                return View();
            }

            if (codigo?.ToUpper() == codigoEsperado)
            {
                return RedirectToAction("RestablecerContrasenna");
            }

            ViewBag.Msj = "Código Incorrecto.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarPerfil(UsuarioModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Autenticacion");

            using var cliente = _httpClient.CreateClient();
            cliente.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var url = _config["ApiUrl"] + "Autenticacion/ActualizarPerfil";

            var response = await cliente.PutAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Msj"] = "Perfil actualizado correctamente.";
            }
            else
            {
                ViewBag.Msj = "No se pudo actualizar el perfil.";
            }

            return RedirectToAction("Index", "Perfil");
        }

        [FiltroSeguridadSesion]
        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Autenticacion");
        }

        #region Cifrar/Descrifrar Contraseñas
        private string Encrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_config.GetSection(_llaveCifrado).Value!);
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
                aes.Key = Encoding.UTF8.GetBytes(_config.GetSection(_llaveCifrado).Value!);
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
