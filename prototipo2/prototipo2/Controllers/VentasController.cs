
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prototipo2.Models;
using prototipo2.Data;

namespace prototipo2.Controllers
{
    public class VentasController : Controller
    {
        private readonly FerreteriaContext _context;

        public VentasController(FerreteriaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ventas = await _context.Venta
                .Include(v => v.Productos)
                .Include(v => v.Pagos)
                .Include(v => v.NotaCredito)
                .Include(v => v.Devoluciones)
                    .ThenInclude(d => d.ProductosDevueltos)
                .ToListAsync();

            return View(ventas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var venta = new Venta();
            venta.Productos.Add(new ItemVendido());
            venta.Pagos.Add(new MetodoPago());
            return View(venta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venta venta)
        {
            // Validaciones manuales
            if (venta.Productos == null || !venta.Productos.Any())
                ModelState.AddModelError("", "Debe agregar al menos un producto.");

            if (venta.Pagos == null || !venta.Pagos.Any())
                ModelState.AddModelError("", "Debe registrar al menos un método de pago.");

            if (venta.Pagos != null && venta.Pagos.GroupBy(p => p.Tipo).Any(g => g.Count() > 1))
                ModelState.AddModelError("", "No se pueden repetir métodos de pago del mismo tipo.");

            // Establecer fecha actual para la venta
            venta.Fecha = DateTime.Now;

            // Filtrar productos válidos (evitar filas vacías)
            venta.Productos = venta.Productos
                .Where(p => !string.IsNullOrWhiteSpace(p.Producto) && p.Cantidad > 0 && p.PrecioUnitario > 0)
                .ToList();

            // Filtrar pagos válidos
            venta.Pagos = venta.Pagos
                .Where(p => !string.IsNullOrWhiteSpace(p.Tipo))
                .ToList();

            // IMPORTANTE: Establecer el Id de venta en productos y pagos para que EF Core los relacione
            foreach (var producto in venta.Productos)
            {
                producto.Venta = venta; // Esto también es opcional pero ayuda a mantener el tracking
            }

            foreach (var pago in venta.Pagos)
            {
                pago.Venta = venta;
            }

            _context.Venta.Add(venta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AgregarDevolucion(int id)
        {
            var venta = await _context.Venta
                .Include(v => v.Devoluciones)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound();

            var devolucion = new Devolucion
            {
                VentaId = venta.Id
            };

            ViewBag.VentaId = venta.Id;
            return View(devolucion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarDevolucion(int id, Devolucion devolucion)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.VentaId = id;
                return View(devolucion);
            }

            devolucion.Id = 0;
            devolucion.Fecha = DateTime.Now;
            devolucion.VentaId = id;

            _context.Devoluciones.Add(devolucion);

            var venta = await _context.Venta
                .Include(v => v.Productos)
                .Include(v => v.Pagos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta != null && venta.NotaCredito == null)
            {
                var montoTotal = venta.Productos.Sum(p => p.Total);
                var nota = new NotaCredito
                {
                    Fecha = DateTime.Now,
                    Monto = montoTotal,
                    Comentario = $"Devolución registrada el {DateTime.Now:dd/MM/yyyy}"
                };

                venta.NotaCredito = nota;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }




        public async Task<IActionResult> Details(int id)
        {
            var venta = await _context.Venta
                .Include(v => v.Productos)
                .Include(v => v.Pagos)
                .Include(v => v.NotaCredito)
                .Include(v => v.Devoluciones)
                    .ThenInclude(d => d.ProductosDevueltos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null) return NotFound();
            return View(venta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var venta = await _context.Venta
                .Include(v => v.Productos)
                .Include(v => v.Pagos)
                .Include(v => v.Devoluciones)
                    .ThenInclude(d => d.ProductosDevueltos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            // Primero eliminar productos devueltos si existen
            foreach (var devolucion in venta.Devoluciones)
            {
                _context.ItemsDevueltos.RemoveRange(devolucion.ProductosDevueltos);
            }

            // Eliminar devoluciones
            _context.Devoluciones.RemoveRange(venta.Devoluciones);

            // Eliminar productos vendidos y pagos
            _context.ItemsVendidos.RemoveRange(venta.Productos);
            _context.Pagos.RemoveRange(venta.Pagos);

            // Finalmente eliminar la venta
            _context.Venta.Remove(venta);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}