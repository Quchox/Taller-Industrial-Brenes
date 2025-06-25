using System.ComponentModel.DataAnnotations;

namespace Taller_Industrial_Brenes_API.Models
{
    public class ProductoCreateModel
    {
        [Required, StringLength(200, MinimumLength = 3)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required, Range(1, long.MaxValue, ErrorMessage = "Debe indicar una categoría válida.")]
        public long CategoriaID { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
        public decimal Precio { get; set; }

        [Required, Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int Cantidad { get; set; }

        public string ImagenUrl { get; set; }
    }
}

