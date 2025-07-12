using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using prototipo2.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Data.SqlClient;

namespace prototipo2.Controllers
{
    public class FinanzasController : Controller
    {
        private readonly string _connectionString;

        public FinanzasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<IActionResult> Index()
        {
            using var db = Connection;
            var movimientos = await db.QueryAsync<MovimientoFinanciero>("SELECT * FROM Finanza WHERE Anulada = 0 ORDER BY Fecha DESC");
            return View(movimientos);
        }

        public IActionResult Create()
        {
            return View(new MovimientoFinanciero());
        }


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

            using var db = Connection;
            var parameters = new DynamicParameters();
            parameters.Add("@Fecha", movimiento.Fecha);
            parameters.Add("@Descripcion", movimiento.Descripcion);
            parameters.Add("@Monto", movimiento.Monto);
            parameters.Add("@Tipo", movimiento.Tipo);
            parameters.Add("@FechaVencimiento", movimiento.FechaVencimiento);
            parameters.Add("@Pagada", movimiento.Pagada);
            parameters.Add("@Anulada", movimiento.Anulada);

            await db.ExecuteAsync("CrearMovimientoFinanciero", parameters, commandType: CommandType.StoredProcedure);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            using var db = Connection;
            var sql = "SELECT * FROM Finanza WHERE Id = @Id";
            var movimiento = await db.QueryFirstOrDefaultAsync<MovimientoFinanciero>(sql, new { Id = id });

            if (movimiento == null) return NotFound();

            return View(movimiento);
        }


        [HttpPost]
        public async Task<IActionResult> MarcarComoPagada(int id)
        {
            using var db = Connection;

            var movimiento = await db.QueryFirstOrDefaultAsync<MovimientoFinanciero>(
                "SELECT * FROM Finanza WHERE Id = @Id", new { Id = id });

            if (movimiento != null && movimiento.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR.ToString())
            {
                await db.ExecuteAsync("MarcarCuentaPorCobrarComoPagada", new { Id = id }, commandType: CommandType.StoredProcedure);
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = Connection;
            var movimiento = await db.QueryFirstOrDefaultAsync<MovimientoFinanciero>("SELECT * FROM Finanza WHERE Id = @Id", new { Id = id });

            if (movimiento == null) return NotFound();

            await db.ExecuteAsync("EliminarMovimientoFinanciero", new { Id = id }, commandType: CommandType.StoredProcedure);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovimientoFinanciero movimiento)
        {
            if (!ModelState.IsValid)
                return View("Details", movimiento);

            using var db = Connection;

            var movOriginal = await db.QueryFirstOrDefaultAsync<MovimientoFinanciero>(
                "SELECT * FROM Finanza WHERE Id = @Id", new { Id = movimiento.Id });

            if (movOriginal == null) return NotFound();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", movimiento.Id);
            parameters.Add("@Fecha", movimiento.Fecha);
            parameters.Add("@Descripcion", movimiento.Descripcion);
            parameters.Add("@Monto", movimiento.Monto);
            parameters.Add("@Tipo", movimiento.Tipo);
            parameters.Add("@FechaVencimiento", movimiento.FechaVencimiento);
            parameters.Add("@Pagada", movimiento.Pagada);
            parameters.Add("@Anulada", movimiento.Anulada);

            await db.ExecuteAsync("ActualizarMovimientoFinanciero", parameters, commandType: CommandType.StoredProcedure);

            return RedirectToAction(nameof(Index));
        }
    }
}