using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;
using System.Linq;

namespace prototipo2.Controllers
{
    public class VentasController : Controller
    {
        private static List<Venta> _ventas = new();
        private static int _nextId = 1;

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
