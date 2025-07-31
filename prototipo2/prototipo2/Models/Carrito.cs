namespace prototipo2.Models
{
    public class CarritoDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre_Producto { get; set; } = null!;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => Precio * Cantidad;
    }
}

