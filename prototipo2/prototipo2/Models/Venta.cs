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
        public int? NotaCreditoId { get; set; }
    }

    public class ItemVendido
    {
        [Key]
        public int Id { get; set; }

        public int VentaId { get; set; }

        public Venta Venta { get; set; }

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
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public string Tipo { get; set; }
        public int VentaId { get; set; }           // FK
        public Venta Venta { get; set; }
    }


    public class Devolucion
    {
        [Key]
        public int Id { get; set; }
        public int VentaId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Motivo { get; set; } = "";
        public List<ItemDevuelto> ProductosDevueltos { get; set; } = new();
    }

    public class ItemDevuelto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Producto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        public string? Observaciones { get; set; }
    }

    public class NotaCredito
    {
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public decimal Monto { get; set; }

        public string? Comentario { get; set; }
    }
}
