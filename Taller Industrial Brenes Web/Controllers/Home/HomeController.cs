using Microsoft.AspNetCore.Mvc;

namespace Taller_Industrial_Brenes_Web.Controllers.Home
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _config;

        public HomeController(IHttpClientFactory httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public IActionResult Inicio()
        {
            return View();
        }

    }
}