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

        public IActionResult Create()
        {
            return View(new Venta
            {
                Productos = new List<ItemVendido> { new ItemVendido() },
                Pagos = new List<MetodoPago> { new MetodoPago() }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venta venta)
        {
            if (!ModelState.IsValid)
                return View(venta);

            venta.Fecha = DateTime.Now;

            // 🔑 Asigna el FK explícito:
            foreach (var p in venta.Productos)
            {
                p.Venta = venta;
            }
            foreach (var pago in venta.Pagos)
            {
                pago.Venta = venta;
            }

            _context.Venta.Add(venta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
    }
}