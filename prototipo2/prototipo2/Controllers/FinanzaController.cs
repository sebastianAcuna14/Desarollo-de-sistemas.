using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class FinanzasController : Controller
    {
        private static List<MovimientoFinanciero> _movimientos = new();
        private static int _nextId = 1;

        // GET: Finanzas
        public IActionResult Index()
        {
            return View(_movimientos);
        }

        // GET: Agregar movimiento manual (ingreso, egreso, cuenta por cobrar)
        public IActionResult Create()
        {
            return View(new MovimientoFinanciero());
        }

        // POST: Crear nuevo movimiento financiero
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MovimientoFinanciero movimiento)
        {
            if (!ModelState.IsValid)
                return View(movimiento);

            movimiento.Id = _nextId++;
            movimiento.Fecha = DateTime.Now;

            if (movimiento.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR)
            {
                movimiento.Pagada = false;
                if (!movimiento.FechaVencimiento.HasValue)
                    movimiento.FechaVencimiento = DateTime.Now.AddDays(30);
            }

            _movimientos.Add(movimiento);
            return RedirectToAction(nameof(Index));
        }

        // GET: Ver detalles
        public IActionResult Details(int id)
        {
            var mov = _movimientos.FirstOrDefault(m => m.Id == id);
            if (mov == null) return NotFound();
            return View(mov);
        }

        // POST: Marcar como pagada una cuenta por cobrar
        [HttpPost]
        public IActionResult MarcarComoPagada(int id)
        {
            var mov = _movimientos.FirstOrDefault(m => m.Id == id && m.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR);
            if (mov != null)
                mov.Pagada = true;

            return RedirectToAction(nameof(Index));
        }

        public static void AgregarMovimientoDesdeVenta(MovimientoFinanciero movimiento)
        {
            movimiento.Id = _nextId++;
            _movimientos.Add(movimiento);
        }
    }
}