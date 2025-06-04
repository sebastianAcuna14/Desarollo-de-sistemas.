using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public required string Nombre_Producto { get; set; }
        [Required(ErrorMessage = "La categoria es obligatoria")]
        public required string Categoria { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        public int Cantidad { get; set; }
        [Required(ErrorMessage = "Ingrese el nombre del proveedor")]
        public required string Proveedor { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        public required decimal Precio { get; set; }
    }
}
