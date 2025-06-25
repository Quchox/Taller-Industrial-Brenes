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
    public class CategoriaController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CategoriaController(IConfiguration config) => _config = config;


        [HttpGet]
        [Route("/ObtenerCategorias")]
        public async Task<IActionResult> ObtenerCategorias()
        {
            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spListarCategorias", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await conn.OpenAsync();

                var lista = new List<CategoriaModel>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    lista.Add(new CategoriaModel
                    {

                        CategoriaID = Convert.ToInt64(reader["CategoriaID"]),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre"))
                    });
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener categorías.", detalle = ex.Message });
            }
        }


        [HttpGet("/ObtenerCategoriaPorID{id:long}")]
        public async Task<IActionResult> ObtenerCategoriaPorID(long id)
        {
            if (id <= 0)
                return BadRequest("El ID de categoría debe ser mayor que cero.");

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spObtenerCategoriaPorId", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CategoriaID", id);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return NotFound(new { mensaje = $"No se encontró ninguna categoría con ID = {id}" });

                var categoria = new CategoriaModel
                {
                    CategoriaID = Convert.ToInt64(reader["CategoriaID"]),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre"))
                };

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la categoría.", detalle = ex.Message });
            }
        }


        [HttpPost("CrearCategoria")]
        public async Task<IActionResult> CrearCategoria([FromBody] CategoriaCreateModel dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("Nombre de categoría requerido.");

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spCrearCategoria", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Nombre", dto.Nombre);

                await conn.OpenAsync();

                var nuevoIdObj = await cmd.ExecuteScalarAsync();
                long nuevoId = Convert.ToInt64(nuevoIdObj);

                var creada = new CategoriaModel
                {
                    CategoriaID = nuevoId,
                    Nombre = dto.Nombre
                };

                return CreatedAtAction(nameof(ObtenerCategoriaPorID),
                                       new { id = nuevoId }, creada);
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la categoría.", detalle = ex.Message });
            }
        }


        [HttpPut("ActualizarCategoria")]
        public async Task<IActionResult> ActualizarCategoria([FromBody] CategoriaModel model)
        {
            if (model == null || model.CategoriaID <= 0 || string.IsNullOrWhiteSpace(model.Nombre))
                return BadRequest("Datos de categoría inválidos.");

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spActualizarCategoria", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CategoriaID", model.CategoriaID);
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);

                await conn.OpenAsync();
                var filas = await cmd.ExecuteNonQueryAsync();

                if (filas == 0)
                    return NotFound(new { mensaje = $"No se encontró categoría con ID = {model.CategoriaID}" });

                return NoContent();
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la categoría.", detalle = ex.Message });
            }
        }


        [HttpDelete("EliminarCategoria{id:long}")]
        public async Task<IActionResult> EliminarCategoria(long id)
        {
            if (id <= 0)
                return BadRequest("El ID de categoría debe ser mayor que cero.");

            try
            {
                var cs = _config.GetConnectionString("ConexionBD");
                using var conn = new SqlConnection(cs);
                using var cmd = new SqlCommand("spEliminarCategoria", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CategoriaID", id);

                await conn.OpenAsync();
                var filas = await cmd.ExecuteNonQueryAsync();

                if (filas == 0)
                    return NotFound(new { mensaje = $"No se encontró categoría con ID = {id}" });

                return NoContent();
            }
            catch (SqlException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar la categoría.", detalle = ex.Message });
            }
        }
    }
}

