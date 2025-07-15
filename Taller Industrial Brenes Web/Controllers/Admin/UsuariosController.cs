using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Taller_Industrial_Brenes_Web.Dependencia;
using Taller_Industrial_Brenes_Web.Models;

namespace Taller_Industrial_Brenes_Web.Controllers.Admin
{

    public class UsuariosController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _config;
        private readonly IUtilitarios _utilitarios;
        private readonly string _apiUrl;

        public UsuariosController(IHttpClientFactory httpClient, IConfiguration config, IUtilitarios utilitarios)
        {
            _httpClient = httpClient;
            _config = config;
            _utilitarios = utilitarios;
            _apiUrl = _config["ApiUrl"];
        }

        [HttpGet]
        public async Task<IActionResult> ListadoAdmin()
        {
            var response = await _utilitarios.ConsultarUsuarioAdmin(0);

            if (response.IsSuccessStatusCode)
            {
                var contenido = await response.Content.ReadAsStringAsync();

                if (contenido.TrimStart().StartsWith("<"))
                {
                    ViewBag.Msj = "La API devolvió HTML en lugar de datos JSON.";
                    return View(new List<UsuarioModel>());
                }

                try
                {
                    var datos = JsonSerializer.Deserialize<List<UsuarioModel>>(contenido);
                    if (datos != null && datos.Any())
                    {
                        return View(datos);
                    }
                    else
                    {
                        ViewBag.Msj = "No hay clientes registrados en este momento.";
                    }
                }
                catch (JsonException)
                {
                    ViewBag.Msj = "Error al deserializar los datos del servidor.";
                }
            }
            else
            {
                ViewBag.Msj = $"Error al consultar clientes: {response.StatusCode}";
            }

            return View(new List<UsuarioModel>());
        }


        [HttpPost]
        public async Task<IActionResult> DesactivarUsuario(long usuarioID)
        {
            using (var api = _httpClient.CreateClient())
            {
                var url = $"{_apiUrl.TrimEnd('/')}/Usuarios/DesactivarUsuario";
                api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                var result = await api.PutAsJsonAsync(url, usuarioID);

                TempData["Msj"] = result.IsSuccessStatusCode
                    ? "El usuario ha sido desactivado correctamente."
                    : "Hubo un error al desactivar el usuario.";
            }

            return RedirectToAction("ListadoAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> ActivarUsuario(long usuarioID)
        {
            using (var api = _httpClient.CreateClient())
            {
                var url = $"{_apiUrl.TrimEnd('/')}/Usuarios/ActivarUsuario";
                api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                var result = await api.PutAsJsonAsync(url, usuarioID);

                TempData["Msj"] = result.IsSuccessStatusCode
                    ? "El usuario ha sido activado correctamente."
                    : "Hubo un error al activar el usuario.";
            }

            return RedirectToAction("ListadoAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarRol(long usuarioID)
        {
            using (var api = _httpClient.CreateClient())
            {
                var url = $"{_apiUrl.TrimEnd('/')}/Usuarios/ActualizarRol";
                api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                var payload = new { UsuarioID = usuarioID };

                var response = await api.PutAsJsonAsync(url, payload);

                if (response.IsSuccessStatusCode)
                {
                    var mensaje = await response.Content.ReadAsStringAsync();
                    TempData["Msj"] = mensaje;
                }
                else
                {
                    TempData["Msj"] = "Error al actualizar el rol.";
                }
            }

            return RedirectToAction("ListadoAdmin");
        }

    }
}