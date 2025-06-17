using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class MovimientoFinanciero
    {
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public TipoMovimiento Tipo { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        public bool Pagada { get; set; } = false;

        public bool Anulada { get; set; } = false;

        public enum TipoMovimiento
        {
            INGRESO,
            EGRESO,
            CUENTA_POR_COBRAR
        }
    }
}