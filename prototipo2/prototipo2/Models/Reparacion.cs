using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Reparacion
    {
        [Key]
        public int IdReparacion { get; set; }
        [Required(ErrorMessage = "El campo Fecha de Salida es obligatorio.")]
        public DateTime Fecha_Ingreso { get; set; }
        [Required(ErrorMessage = "El campo Fecha de Ingreso es obligatorio.")]
        public DateTime Fecha_Salida { get; set; }
        [Required(ErrorMessage = "El campo Descripción es obligatorio.")]
        public required string Descripcion { get; set; }
        [Required(ErrorMessage = "El campo Cossto es obligatorio.")]
        public required string Estado { get; set; }
        [Required(ErrorMessage = "El campo Costo es obligatorio.")]
        public required string IdCliente { get; set; } // llave foranea de la cedula del cliente
    }
}

