using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public required string Nombre_Producto { get; set; }
        [Required(ErrorMessage = "La categoria es obligatoria")]
        public required string Numero_Pedido { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        public int Cantidad { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La fecha del pedido es obligatoria")]
        public DateTime FechaPedido { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        public required decimal Precio { get; set; }

        [Required(ErrorMessage = "El estado del pedido es obligatorio")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public required string Estado { get; set; }
    }
}
