namespace Taller_Industrial_Brenes_API.Models
{
    public class ProductoModel
    {
        public long ProductoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public long CategoriaID { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }  
        public string ImagenUrl { get; set; }
        public bool EstaActivo { get; set; }
    }
}


