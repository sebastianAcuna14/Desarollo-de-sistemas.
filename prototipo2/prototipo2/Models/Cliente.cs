using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idCliente { get; set; }

        [Required]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string? Apellido { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Correo { get; set; }

        [StringLength(20)]
        public string? Telefonos { get; set; }

        [StringLength(100)]
        public string? Contrasena { get; set; }

        [StringLength(20)]
        public string? Cedula { get; set; }

        [StringLength(50)]
        public string? Estado { get; set; } = "Activo";

        [StringLength(255)]
        public string? Direccion { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? FechaModificacion { get; set; }

        public string? Token { get; set; }
    }
}