using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Taller_Industrial_Brenes_Web.Models;

namespace Taller_Industrial_Brenes_Web.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClient;
        private readonly string _apiUrl;
        public AutenticacionController(IConfiguration config, IHttpClientFactory httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _apiUrl = _config["ApiUrl"];
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
    
    }
}
