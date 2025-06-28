using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public required string Nombre { get; set; }

        [StringLength(255, ErrorMessage = "La descripción no puede exceder los 255 caracteres.")]
        public string? Descripcion { get; set; } //signo de interrogaccion (?) para que sea opcional 
    }





}
