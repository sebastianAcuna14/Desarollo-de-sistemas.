using System.ComponentModel.DataAnnotations;

namespace prototipo2.Models
{
    public class Venta
    {
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public List<ItemVendido> Productos { get; set; } = new();

        public List<MetodoPago> Pagos { get; set; } = new();

        public List<Devolucion> Devoluciones { get; set; } = new();

        public NotaCredito? NotaCredito { get; set; }
    }

    public class ItemVendido
    {
        [Required]
        public string Producto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        public decimal Total => Cantidad * PrecioUnitario;
    }

    public class MetodoPago
    {
        public enum TipoPago { EFECTIVO, TARJETA, SINPE }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public TipoPago Tipo { get; set; }  
    }

    public class Devolucion
    {
        public DateTime Fecha { get; set; } = DateTime.Now;

        public string Motivo { get; set; } = "";

        public List<ItemDevuelto> ProductosDevueltos { get; set; } = new();
    }

    public class ItemDevuelto
    {
        [Required]
        public string Producto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        public string? Observaciones { get; set; }
    }

    public class NotaCredito
    {
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public decimal Monto { get; set; }

        public string? Comentario { get; set; }
    }
}
