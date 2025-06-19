using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class FinanzasController : Controller
    {
        private static List<MovimientoFinanciero> _movimientos = new()
{
    new MovimientoFinanciero
    {
        Id = 1,
        Fecha = DateTime.Now.AddDays(-2),
        Descripcion = "Venta de producto A",
        Monto = 150000,
        Tipo = MovimientoFinanciero.TipoMovimiento.INGRESO,
        Pagada = true
    },
    new MovimientoFinanciero
    {
        Id = 2,
        Fecha = DateTime.Now,
        Descripcion = "Cuenta por cobrar - Cliente Juan Pérez",
        Monto = 85000,
        Tipo = MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR,
        Pagada = false,
        FechaVencimiento = DateTime.Now.AddDays(15)
    }
};

        private static int _nextId = 3;


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

        [HttpPost]
        public IActionResult MarcarComoPagada(int id)
        {
            var mov = _movimientos.FirstOrDefault(m => m.Id == id && m.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR);
            if (mov != null)
            {
                mov.Pagada = true;
                mov.Tipo = MovimientoFinanciero.TipoMovimiento.INGRESO; 
            }

            return RedirectToAction(nameof(Index));
        }



        public static void AgregarMovimientoDesdeVenta(MovimientoFinanciero movimiento)
        {
            movimiento.Id = _nextId++;
            _movimientos.Add(movimiento);
        }

        [HttpPost]
        public IActionResult Anular(int id)
        {
            var mov = _movimientos.FirstOrDefault(m => m.Id == id);
            if (mov != null && !mov.Anulada)
                mov.Anulada = true;

            return RedirectToAction(nameof(Index));
        }

    }
}