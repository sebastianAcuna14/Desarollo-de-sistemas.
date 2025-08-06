using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Empleado
    {
        [Key]
        public int IdEmpleado { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre completo no debe exceder los 100 caracteres")]
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Correo { get; set; }
        public string? Telefono { get; set; }
        public int IdRol { get; set; }


        [Required(ErrorMessage = "La cédula es obligatoria")]
        [RegularExpression(@"^\d{9,12}$", ErrorMessage = "La cédula debe tener entre 9 y 12 dígitos")]
        public required string Cedula { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        [StringLength(50)]
        public required string Rol { get; set; }
        public string? Contrasena { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "El salario debe ser mayor que cero")]
        public required decimal Salario { get; set; }

        [Required(ErrorMessage = "La fecha de contratación es obligatoria")]
        [DataType(DataType.Date)]
        public required DateTime FechaContratacion { get; set; }
        public string? Token { get; set; }


    }
}

