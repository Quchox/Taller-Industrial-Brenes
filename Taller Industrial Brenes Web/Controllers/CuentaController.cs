using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Taller_Industrial_Brenes_Web.Models;

namespace Taller_Industrial_Brenes_Web.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public CuentaController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["ApiUrl"];
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync($"{_apiUrl}/Autenticacion/Login", model);

            if (response.IsSuccessStatusCode)
            {
                var usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();
                TempData["UsuarioNombre"] = usuario?.Nombre;
                TempData["RolID"] = usuario?.RolID.ToString(); 
                TempData.Keep("RolID"); 
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Correo o contraseña inválidos";
            return View();
        }

        [HttpGet]
        public IActionResult Registro() => View();

        [HttpPost]
        public async Task<IActionResult> Registro(UsuarioModel model)
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync($"{_apiUrl}/Autenticacion/Registro", model);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            ViewBag.Error = "Error al registrar el usuario";
            return View();
        }
    }
}
