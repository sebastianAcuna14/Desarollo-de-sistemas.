using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }

        [Required]
        public string? Nombre { get; set; }

        [Required]
        public string? Descripcion { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int IdProveedor { get; set; }

        public string? NombreProveedor { get; set; }

        [Required]
        public int IdCategoria { get; set; }
        public string? NombreCategoria { get; set; }
    }
}
