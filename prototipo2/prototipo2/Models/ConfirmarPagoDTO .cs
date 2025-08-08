namespace prototipo2.Models
{
    public class ConfirmarPagoDTO
    {
        public string orderId { get; set; } = null!;
        public string payerId { get; set; } = null!;
        public string contacto { get; set; } = null!;
        public string telefono { get; set; } = null!;
        public string direccion { get; set; } = null!;
        public int IdProvincia { get; set; }
        public int IdDepartamento { get; set; }
        public int IdDistrito { get; set; }
        public decimal total { get; set; }

    }
}
