using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Proveedor
    {
        public int IDProveedor { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        public string? NombreEmpresa { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string? Correo { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string? Telefono { get; set; }

        public string Estado { get; set; } = "Activo";
    }
}
