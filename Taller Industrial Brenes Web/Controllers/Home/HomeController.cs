using Microsoft.AspNetCore.Mvc;
using Taller_Industrial_Brenes_Web.Models;
using Microsoft.Extensions.Configuration;

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
            public IActionResult Index()
            {
                return View();
            }
        
    }
}
