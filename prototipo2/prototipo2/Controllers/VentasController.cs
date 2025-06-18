using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;
using System.Linq;

namespace prototipo2.Controllers
{
    public class VentasController : Controller
    {
        private static int _nextId = 1;


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
            new MetodoPago { Monto = 9500 }
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
            new MetodoPago { Monto = 3600 }
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
                Productos = new List<ItemVendido>(),
                Pagos = new List<MetodoPago>()
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

            if (!ModelState.IsValid)
                return View(venta);

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
            return View(new Devolucion
            {
                ProductosDevueltos = new List<ItemDevuelto>()
            });
        }

        // POST: /Ventas/AgregarDevolucion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarDevolucion(int id, Devolucion devolucion)
        {
            var venta = _ventas.FirstOrDefault(v => v.Id == id);
            if (venta == null)
                return NotFound();

            if (devolucion.ProductosDevueltos == null || !devolucion.ProductosDevueltos.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un producto para devolver.");
            }


            if ((DateTime.Now - venta.Fecha).TotalDays > 30)
            {
                ModelState.AddModelError("", "El plazo para devolución ha expirado.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.VentaId = id;
                return View(devolucion);
            }

            devolucion.Fecha = DateTime.Now;
            venta.Devoluciones.Add(devolucion);

            decimal totalDevuelto = CalcularTotalDevuelto(venta, devolucion);

            venta.NotaCredito = new NotaCredito
            {
                Fecha = DateTime.Now,
                Monto = totalDevuelto,
                Comentario = $"Reembolso por devolución del {DateTime.Now:dd/MM/yyyy}"
            };

            FinanzasController.AgregarMovimientoDesdeVenta(new MovimientoFinanciero
            {
                Fecha = DateTime.Now,
                Descripcion = $"Devolución Venta #{venta.Id}",
                Monto = -totalDevuelto,
                Tipo = MovimientoFinanciero.TipoMovimiento.EGRESO
            });

            return RedirectToAction(nameof(Details), new { id });
        }

        private decimal CalcularTotalDevuelto(Venta venta, Devolucion devolucion)
        {
            decimal total = 0;
            foreach (var item in devolucion.ProductosDevueltos)
            {
                var productoOriginal = venta.Productos.FirstOrDefault(p => p.Producto == item.Producto);
                if (productoOriginal != null)
                    total += item.Cantidad * productoOriginal.PrecioUnitario;
            }
            return total;
        }
    }
}
