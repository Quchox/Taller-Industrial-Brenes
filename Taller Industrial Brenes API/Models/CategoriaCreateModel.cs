using System.ComponentModel.DataAnnotations;

namespace Taller_Industrial_Brenes_API.Models
{
    public class CategoriaCreateModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; }
    }
}

