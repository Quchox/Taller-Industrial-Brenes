
using Microsoft.AspNetCore.Mvc;
using Taller_Industrial_Brenes_Web.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Taller_Industrial_Brenes_Web.Controllers
{
    public class HorariosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public HorariosController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Administrar");
        }


        public async Task<IActionResult> Administrar()
        {
            var cliente = _httpClientFactory.CreateClient();
            var response = await cliente.GetAsync(_config["ApiUrl"] + "/Horario/Todos");

            if (!response.IsSuccessStatusCode)
                return View("Error", new ErrorViewModel());

            var contenido = await response.Content.ReadAsStringAsync();
            var horarios = JsonSerializer.Deserialize<List<HorarioModel>>(contenido, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(horarios);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(HorarioModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cliente = _httpClientFactory.CreateClient();
            var contenido = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await cliente.PostAsync(_config["ApiUrl"] + "/Horario/Crear", contenido);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            return RedirectToAction("Administrar");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var response = await cliente.GetAsync(_config["ApiUrl"] + $"/Horario/PorID/{id}");

            if (!response.IsSuccessStatusCode)
                return View("Error", new ErrorViewModel());

            var contenido = await response.Content.ReadAsStringAsync();
            var horario = JsonSerializer.Deserialize<HorarioModel>(contenido, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(horario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(HorarioModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cliente = _httpClientFactory.CreateClient();
            var contenido = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await cliente.PutAsync(_config["ApiUrl"] + "/Horario/Actualizar", contenido);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            return RedirectToAction("Administrar");
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var response = await cliente.GetAsync(_config["ApiUrl"] + $"/Horario/PorID/{id}");

            if (!response.IsSuccessStatusCode)
                return View("Error", new ErrorViewModel());

            var contenido = await response.Content.ReadAsStringAsync();
            var horario = JsonSerializer.Deserialize<HorarioModel>(contenido, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(horario);
        }

        [HttpPost, ActionName("Eliminar")]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var response = await cliente.DeleteAsync(_config["ApiUrl"] + $"/Horario/Eliminar/{id}");

            return RedirectToAction("Administrar");
        }


        //EMPELADO 
        public async Task<IActionResult> MiHorario()
        {
            // Simulación de ID del empleado actual (esto se reemplaza luego con el login real)
            long usuarioId = 6;

            var cliente = _httpClientFactory.CreateClient();
            var response = await cliente.GetAsync(_config["ApiUrl"] + $"/Horario/PorUsuario/{usuarioId}");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var contenido = await response.Content.ReadAsStringAsync();
            var horarios = JsonSerializer.Deserialize<List<HorarioModel>>(contenido, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(horarios);
        }

        /* public async Task<IActionResult> MiHorario()
         {
             var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

             if (string.IsNullOrEmpty(usuarioIdStr))
                 return RedirectToAction("Login", "Cuenta"); 

             long usuarioId = long.Parse(usuarioIdStr); 

             var cliente = _httpClientFactory.CreateClient();
             var response = await cliente.GetAsync(_config["ApiUrl"] + $"/Horario/PorUsuario/{usuarioId}");

             if (!response.IsSuccessStatusCode)
                 return View("Error");

             var contenido = await response.Content.ReadAsStringAsync();
             var horarios = JsonSerializer.Deserialize<List<HorarioModel>>(contenido, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

             return View(horarios);
         }*/



    }
}
