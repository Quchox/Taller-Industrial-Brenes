using Microsoft.AspNetCore.Mvc;
using Taller_Industrial_Brenes_Web.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Taller_Industrial_Brenes_Web.Controllers
{
    public class InventarioController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public InventarioController(IConfiguration config)
        {
            _config = config;
            _apiUrl = _config["ApiUrl"];
        }

        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            var productos = await client.GetFromJsonAsync<List<ProductoModel>>($"{_apiUrl}/Inventario/ObtenerProductos");

            return View(productos);
        }

        public async Task<IActionResult> Alertas()
        {
            using var client = new HttpClient();
            var productos = await client.GetFromJsonAsync<List<ProductoModel>>($"{_apiUrl}/Inventario/ProductosStockBajo");

            return View(productos);
        }
        [HttpGet]
        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(ProductoModel producto)
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync($"{_apiUrl}/Inventario/AgregarProducto", producto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Hubo un error al registrar el producto.";
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(long productoID)
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync($"{_apiUrl}/Inventario/EliminarProducto", productoID);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Producto eliminado correctamente.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "No se pudo eliminar el producto.";
            return RedirectToAction("Index");
        }


    }
}
