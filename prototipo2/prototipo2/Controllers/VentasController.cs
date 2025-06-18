using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;
using System.Linq;

namespace prototipo2.Controllers
{
    public class VentasController : Controller
    {
        private static int _nextId = 3;

        private static List<Venta> _ventas = new()
        {
            new Venta
            {
                Id = 1,
                Fecha = DateTime.Now.AddDays(-3),
                Productos = new List<ItemVendido>
                {
                    new ItemVendido { Producto = "Taladro", Cantidad = 2, PrecioUnitario = 2500 },
                    new ItemVendido { Producto = "Desatornillador", Cantidad = 1, PrecioUnitario = 4500 }
                },
                Pagos = new List<MetodoPago>
                {
                    new MetodoPago { Tipo = MetodoPago.TipoPago.EFECTIVO, Monto = 9500 }
                },
                Devoluciones = new List<Devolucion>(),
                NotaCredito = null
            },
            new Venta
            {
                Id = 2,
                Fecha = DateTime.Now.AddDays(-1),
                Productos = new List<ItemVendido>
                {
                    new ItemVendido { Producto = "Sierra", Cantidad = 3, PrecioUnitario = 1200 }
                },
                Pagos = new List<MetodoPago>
                {
                    new MetodoPago { Tipo = MetodoPago.TipoPago.TARJETA, Monto = 3600 }
                },
                Devoluciones = new List<Devolucion>
                {
                    new Devolucion
                    {
                        Fecha = DateTime.Now.AddDays(-1),
                        Motivo = "Producto vencido",
                        ProductosDevueltos = new List<ItemDevuelto>
                        {
                            new ItemDevuelto { Producto = "Conjunto de llaves inglesas", Cantidad = 1, Observaciones = "Cliente mostró recibo" }
                        }
                    }
                },
                NotaCredito = new NotaCredito
                {
                    Fecha = DateTime.Now.AddDays(-1),
                    Monto = 1200,
                    Comentario = "Algunas Llaves venian rotas"
                }
            }
        };


        // GET: /Ventas
        public IActionResult Index()
        {
            return View(_ventas);
        }

        // GET: /Ventas/Create
        public IActionResult Create()
        {
            return View(new Venta
            {
                Productos = new List<ItemVendido> { new ItemVendido() },
                Pagos = new List<MetodoPago> { new MetodoPago() }
            });
        }

        // POST: /Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Venta venta)
        {
            if (venta.Productos == null || !venta.Productos.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un producto.");
            }

            if (venta.Pagos == null || !venta.Pagos.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un método de pago.");
            }

            if (!ModelState.IsValid)
                return View(venta);

            // Calcular total de la venta
            decimal totalVenta = venta.Productos.Sum(p => p.Cantidad * p.PrecioUnitario);

            // Distribuir el monto total entre los métodos de pago igualitariamente
            int pagosCount = venta.Pagos.Count;
            decimal montoPorPago = totalVenta / pagosCount;

            foreach (var pago in venta.Pagos)
            {
                pago.Monto = montoPorPago;
            }

            venta.Id = _nextId++;
            venta.Fecha = DateTime.Now;
            venta.Devoluciones = new List<Devolucion>();
            venta.NotaCredito = null;

            _ventas.Add(venta);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Ventas/Details/5
        public IActionResult Details(int id)
        {
            var venta = _ventas.FirstOrDefault(v => v.Id == id);
            if (venta == null)
                return NotFound();

            return View(venta);
        }

        // GET: /Ventas/AgregarDevolucion/5
        public IActionResult AgregarDevolucion(int id)
        {
            var venta = _ventas.FirstOrDefault(v => v.Id == id);
            if (venta == null)
                return NotFound();

            ViewBag.VentaId = id;

            // Inicializar con los productos de la venta para ayudar a seleccionar devoluciones
            var devolucion = new Devolucion
            {
                ProductosDevueltos = venta.Productos.Select(p => new ItemDevuelto
                {
                    Producto = p.Producto,
                    Cantidad = 0,
                    Observaciones = ""
                }).ToList()
            };

            return View(devolucion);
        }

        // POST: /Ventas/AgregarDevolucion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarDevolucion(int id, Devolucion devolucion)
        {
            var venta = _ventas.FirstOrDefault(v => v.Id == id);
            if (venta == null)
                return NotFound();


            // Validar plazo de devolución (máx 30 días)
            if ((DateTime.Now - venta.Fecha).TotalDays > 30)
            {
                ModelState.AddModelError("", "El plazo para devolución ha expirado.");
            }

            // Validar que la cantidad devuelta no supere la vendida
            foreach (var itemDevuelto in devolucion.ProductosDevueltos.Where(p => p.Cantidad > 0))
            {
                var productoVendido = venta.Productos.FirstOrDefault(p => p.Producto == itemDevuelto.Producto);
                if (productoVendido == null || itemDevuelto.Cantidad > productoVendido.Cantidad)
                {
                    ModelState.AddModelError("", $"La cantidad a devolver de {itemDevuelto.Producto} no puede ser mayor que la vendida.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.VentaId = id;
                return View(devolucion);
            }

            devolucion.Fecha = DateTime.Now;
            venta.Devoluciones.Add(devolucion);

            decimal totalDevuelto = venta.Productos.Sum(p => p.Cantidad * p.PrecioUnitario);


            venta.NotaCredito = new NotaCredito
            {
                Fecha = DateTime.Now,
                Monto = totalDevuelto,
                Comentario = devolucion.Motivo
            };



            FinanzasController.AgregarMovimientoDesdeVenta(new MovimientoFinanciero
            {
                Fecha = DateTime.Now,
                Descripcion = $"Devolución Venta #{venta.Id}",
                Monto = -totalDevuelto,
                Tipo = MovimientoFinanciero.TipoMovimiento.EGRESO
            });

            return RedirectToAction(nameof(Index));

        }
    }
}
