using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Taller_Industrial_Brenes_API.Models;

namespace Taller_Industrial_Brenes_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ProductoController(IConfiguration config) => _config = config;

        

        [HttpGet]
        [Route("/ObtenerProductos")]
        public async Task<IActionResult> ObtenerProductos()
        {
            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spListarProductos", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await conn.OpenAsync();

                var lista = new List<ProductoModel>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    lista.Add(MapProducto(reader));

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al listar productos.", detalle = ex.Message });
            }
        }

        

        [HttpGet]
        [Route("/ObtenerProductosPorCategoria/{categoriaId:long}")]
        public async Task<IActionResult> ObtenerProductosPorCategoria(long categoriaId)
        {
            if (categoriaId <= 0)
                return BadRequest("El ID de categoría debe ser mayor que cero.");

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spListarProductosPorCategoria", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CategoriaId", categoriaId);

                await conn.OpenAsync();

                var lista = new List<ProductoModel>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    lista.Add(MapProducto(reader));

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al listar productos por categoría.", detalle = ex.Message });
            }
        }

       

        [HttpGet]
        [Route("/ObtenerProductoPorID/{id:long}")]
        public async Task<IActionResult> ObtenerProductoPorID(long id)
        {
            if (id <= 0)
                return BadRequest("El ID de producto debe ser mayor que cero.");

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spObtenerProductoPorId", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductoId", id);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return NotFound(new { mensaje = $"No se encontró producto con ID = {id}" });

                var prod = MapProducto(reader);
                prod.EstaActivo = reader.GetBoolean(reader.GetOrdinal("EstaActivo"));

                return Ok(prod);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener producto.", detalle = ex.Message });
            }
        }



        [HttpPost("CrearProducto")]
        public async Task<IActionResult> CrearProducto([FromBody] ProductoCreateModel dto)
        {
             if (!ModelState.IsValid)
                return BadRequest(ModelState);    

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spCrearProducto", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Nombre", dto.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object?)dto.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoriaId", dto.CategoriaID);
                cmd.Parameters.AddWithValue("@Precio", dto.Precio);
                cmd.Parameters.AddWithValue("@Cantidad", dto.Cantidad);
                cmd.Parameters.AddWithValue("@ImagenUrl", (object?)dto.ImagenUrl ?? DBNull.Value);

                await conn.OpenAsync();
                var nuevoIdObj = await cmd.ExecuteScalarAsync();           
                long nuevoId = Convert.ToInt64(nuevoIdObj);             

                var creado = new ProductoModel
                {
                    ProductoID = nuevoId,
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    CategoriaID = dto.CategoriaID,
                    Precio = dto.Precio,
                    Cantidad = dto.Cantidad,
                    ImagenUrl = dto.ImagenUrl,
                    EstaActivo = true
                };

                return CreatedAtAction(nameof(ObtenerProductoPorID),
                                       new { id = nuevoId },
                                       creado);
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear producto.", detalle = ex.Message });
            }
        }





        [HttpPut("ActualizarProducto")]
        public async Task<IActionResult> ActualizarProducto([FromBody] ProductoModel model)
        {
            if (model == null || model.ProductoID <= 0 || model.Cantidad < 0)
                return BadRequest("Datos de producto inválidos.");

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spActualizarProducto", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ProductoId", model.ProductoID);
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object?)model.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoriaId", model.CategoriaID);
                cmd.Parameters.AddWithValue("@Precio", model.Precio);
                cmd.Parameters.AddWithValue("@Cantidad", model.Cantidad);  

                cmd.Parameters.AddWithValue("@ImagenUrl", (object?)model.ImagenUrl ?? DBNull.Value);

                await conn.OpenAsync();
                var filas = await cmd.ExecuteNonQueryAsync();

                if (filas == 0)
                    return NotFound(new { mensaje = $"No se encontró producto con ID = {model.ProductoID}" });

                return NoContent();
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar producto.", detalle = ex.Message });
            }
        }

       

        [HttpDelete("EliminarProducto/{id:long}")]
        public async Task<IActionResult> EliminarProducto(long id)
        {
            if (id <= 0)
                return BadRequest("El ID de producto debe ser mayor que cero.");

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spEliminarProducto", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductoId", id);

                await conn.OpenAsync();
                var filas = await cmd.ExecuteNonQueryAsync();

                if (filas == 0)
                    return NotFound(new { mensaje = $"No se encontró producto con ID = {id}" });

                return NoContent();
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar producto.", detalle = ex.Message });
            }
        }

       

        private static ProductoModel MapProducto(SqlDataReader r) =>
            new ProductoModel
            {
                ProductoID = Convert.ToInt64(r["ProductoId"]),
                Nombre = r.GetString(r.GetOrdinal("Nombre")),
                Descripcion = r.IsDBNull(r.GetOrdinal("Descripcion"))
                                ? null
                                : r.GetString(r.GetOrdinal("Descripcion")),
                CategoriaID = Convert.ToInt64(r["CategoriaId"]),
                Precio = r.GetDecimal(r.GetOrdinal("Precio")),
                Cantidad = r.GetInt32(r.GetOrdinal("Cantidad")),  // nuevo
                ImagenUrl = r.IsDBNull(r.GetOrdinal("ImagenUrl"))
                                ? null
                                : r.GetString(r.GetOrdinal("ImagenUrl")),
                EstaActivo = true
            };
    }
}


