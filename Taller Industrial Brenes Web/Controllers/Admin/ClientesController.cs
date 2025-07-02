using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Taller_Industrial_Brenes_Web.Dependencia;
using Taller_Industrial_Brenes_Web.Models;

namespace Taller_Industrial_Brenes_Web.Controllers.Admin
{

    public class ClientesController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _config;
        private readonly IUtilitarios _utilitarios;
        private readonly string _apiUrl;

        public ClientesController(IHttpClientFactory httpClient, IConfiguration config, IUtilitarios utilitarios)
        {
            _httpClient = httpClient;
            _config = config;
            _utilitarios = utilitarios;
            _apiUrl = _config["ApiUrl"];
        }

        [HttpGet]
        public IActionResult ListadoAdmin()
        {
            var response = _utilitarios.ConsultarClientesAdmin(0);

            if (response.IsSuccessStatusCode)
            {
                var datos = response.Content.ReadFromJsonAsync<List<UsuarioModel>>().Result;

                if (datos != null && datos.Any())
                {
                    return View(datos);
                }
                else
                {
                    ViewBag.Msj = "No hay clientes registrados en este momento";
                }
            }
            else
            {
                ViewBag.Msj = "No se pudo completar su petición";
            }

            return View(new List<UsuarioModel>());
        }

        [HttpPost]
        public IActionResult DesactivarUsuario(long usuarioID)
        {
            using (var api = _httpClient.CreateClient())
            {
                var url = _config.GetSection(_apiUrl).Value + "Clientes/DesactivarUsuario";

                api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var result = api.PutAsJsonAsync(url, usuarioID).Result;

                if (result.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "El usuario ha sido desactivado correctamente.";
                }
                else
                {
                    ViewBag.Mensaje = "Hubo un error al desactivar el usuario.";
                }
            }

            return RedirectToAction("ListadoAdmin");
        }

        [HttpPost]
        public IActionResult ActivarUsuario(long usuarioID)
        {
            using (var api = _httpClient.CreateClient())
            {
                var url = _config.GetSection(_apiUrl).Value + "Clientes/ActivarUsuario";

                api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var result = api.PutAsJsonAsync(url, usuarioID).Result;

                if (result.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "El usuario ha sido activado correctamente.";
                }
                else
                {
                    ViewBag.Mensaje = "Hubo un error al activar el usuario.";
                }
            }

            return RedirectToAction("ListadoAdmin");
        }
    }
}

