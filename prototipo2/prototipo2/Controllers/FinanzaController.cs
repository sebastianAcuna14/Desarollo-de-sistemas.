using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prototipo2.Models;
using prototipo2.Data;

namespace prototipo2.Controllers
{
    public class FinanzasController : Controller
    {
        private readonly FerreteriaContext _context;

        public FinanzasController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: Finanzas
        public async Task<IActionResult> Index()
        {
            var movimientos = await _context.Finanza.ToListAsync();
            return View(movimientos);
        }

        // GET: Finanzas/Create
        public IActionResult Create()
        {
            return View(new MovimientoFinanciero());
        }

        // POST: Finanzas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovimientoFinanciero movimiento)
        {
            if (!ModelState.IsValid)
                return View(movimiento);

            movimiento.Fecha = DateTime.Now;

            if (movimiento.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR.ToString())
            {
                movimiento.Pagada = false;
                if (!movimiento.FechaVencimiento.HasValue)
                    movimiento.FechaVencimiento = DateTime.Now.AddDays(30);
            }
            else if (movimiento.Tipo == MovimientoFinanciero.TipoMovimiento.INGRESO.ToString())
            {
                movimiento.Pagada = true;
            }

            _context.Finanza.Add(movimiento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var mov = await _context.Finanza.FindAsync(id);
            if (mov == null) return NotFound();
            return View(mov);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoPagada(int id)
        {
            var mov = await _context.Finanza.FindAsync(id);
            if (mov != null && mov.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR.ToString())
            {
                mov.Pagada = true;
                mov.Tipo = MovimientoFinanciero.TipoMovimiento.INGRESO.ToString();
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            var mov = await _context.Finanza.FindAsync(id);
            if (mov != null && !mov.Anulada)
            {
                mov.Anulada = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}