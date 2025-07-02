using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Taller_Industrial_Brenes_Web.Models;

namespace Taller_Industrial_Brenes_Web.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public AsistenciaController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public IActionResult Marcar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MarcarEntrada()
        {
            var cliente = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(new { UsuarioID = 6, HoraEntrada = DateTime.Now.ToString("HH:mm:ss") });
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await cliente.PostAsync(_config["ApiUrl"] + "/api/Asistencia/marcar-entrada", contenido);

            if (!response.IsSuccessStatusCode)
                ModelState.AddModelError(string.Empty, "Error al marcar entrada.");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> MarcarSalida()
        {
            var cliente = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(new { UsuarioID = 6, HoraSalida = DateTime.Now.ToString("HH:mm:ss") });
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await cliente.PostAsync(_config["ApiUrl"] + "/api/Asistencia/marcar-salida", contenido);

            if (!response.IsSuccessStatusCode)
                ModelState.AddModelError(string.Empty, "Error al marcar salida.");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Historial()
        {
            var cliente = _httpClientFactory.CreateClient();
            var response = await cliente.GetAsync(_config["ApiUrl"] + "/api/Asistencia/usuario/6");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var contenido = await response.Content.ReadAsStringAsync();
            var lista = JsonSerializer.Deserialize<List<AsistenciaModel>>(contenido, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(lista);
        }
    }
}
