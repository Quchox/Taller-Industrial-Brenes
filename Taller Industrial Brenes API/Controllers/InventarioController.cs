using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using Taller_Industrial_Brenes_API.Models;

namespace Taller_Industrial_Brenes_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InventarioController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("ObtenerProductos")]
        public IActionResult ObtenerProductos()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("ConexionBD"));

            var productos = connection.Query<ProductoModel>(
                "ObtenerProductos",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(productos);
        }

        [HttpGet("ProductosStockBajo")]
        public IActionResult ProductosStockBajo()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("ConexionBD"));

            var productos = connection.Query<ProductoModel>(
                "ObtenerProductosConStockBajo",
                new { Limite = 10 },
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(productos);
        }

        [HttpPost("AgregarProducto")]
        public IActionResult AgregarProducto([FromBody] ProductoModel producto)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("ConexionBD"));

            connection.Execute(
                "InsertarProducto",
                new
                {
                    producto.Nombre,
                    producto.Descripcion,
                    producto.Stock,
                    producto.Precio
                },
                commandType: System.Data.CommandType.StoredProcedure
            );

            return Ok("Producto agregado exitosamente");
        }

        [HttpPost("EliminarProducto")]
        public IActionResult EliminarProducto([FromBody] long productoID)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("ConexionBD"));

            connection.Execute(
                "EliminarProducto",
                new { ProductoID = productoID },
                commandType: System.Data.CommandType.StoredProcedure
            );

            return Ok("Producto eliminado correctamente");
        }

    }
}
