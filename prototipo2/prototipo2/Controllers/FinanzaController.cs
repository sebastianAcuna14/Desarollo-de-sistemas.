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
using ClosedXML.Excel;
using System.IO;

namespace prototipo2.Controllers
{
    public class FinanzasController : Controller
    {
        private readonly string _connectionString;

        public FinanzasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Connection");
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

        public async Task<IActionResult> GenerarReporte()
        {
            using var db = Connection;
            var movimientos = await db.QueryAsync<MovimientoFinanciero>("SELECT * FROM Finanza WHERE Anulada = 0");

            var reporte = new ReporteFinanciero
            {
                TotalIngresos = movimientos
                    .Where(m => m.Tipo == MovimientoFinanciero.TipoMovimiento.INGRESO.ToString())
                    .Sum(m => m.Monto),

                TotalEgresos = movimientos
                    .Where(m => m.Tipo == MovimientoFinanciero.TipoMovimiento.EGRESO.ToString())
                    .Sum(m => m.Monto),

                TotalCuentasPorCobrar = movimientos
                    .Where(m => m.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR.ToString() && !m.Pagada)
                    .Sum(m => m.Monto),

                TotalCuentasPorCobrarVencidas = movimientos
                    .Where(m => m.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR.ToString() &&
                                !m.Pagada && m.FechaVencimiento < DateTime.Now)
                    .Sum(m => m.Monto),

                CantPendientes = movimientos
                    .Count(m => m.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR.ToString() && !m.Pagada),

                CantPendientesVencidas = movimientos
                    .Count(m => m.Tipo == MovimientoFinanciero.TipoMovimiento.CUENTA_POR_COBRAR.ToString() &&
                                !m.Pagada && m.FechaVencimiento < DateTime.Now)
            };

            return PartialView("_ReporteFinanciero", reporte);
        }

        public async Task<IActionResult> ExportarReporteExcel()
        {
            using var db = Connection;
            var movimientos = await db.QueryAsync<MovimientoFinanciero>("SELECT * FROM Finanza WHERE Anulada = 0 ORDER BY Fecha DESC");

            var totalIngresos = movimientos.Where(m => m.Tipo == "INGRESO").Sum(m => m.Monto);
            var totalEgresos = movimientos.Where(m => m.Tipo == "EGRESO").Sum(m => m.Monto);
            var cuentasPorCobrar = movimientos.Where(m => m.Tipo == "CUENTA_POR_COBRAR");
            var totalCuentasPorCobrar = cuentasPorCobrar.Sum(m => m.Monto);
            var totalCuentasPorCobrarVencidas = cuentasPorCobrar.Where(m => m.FechaVencimiento < DateTime.Now && !m.Pagada).Sum(m => m.Monto);
            var cantPendientes = cuentasPorCobrar.Count(m => !m.Pagada);
            var cantPendientesVencidas = cuentasPorCobrar.Count(m => !m.Pagada && m.FechaVencimiento < DateTime.Now);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte Financiero");

            // Títulos
            worksheet.Cell(1, 1).Value = "Concepto";
            worksheet.Cell(1, 2).Value = "Valor";

            // Datos
            worksheet.Cell(2, 1).Value = "Total Ingresos";
            worksheet.Cell(2, 2).Value = totalIngresos;

            worksheet.Cell(3, 1).Value = "Total Egresos";
            worksheet.Cell(3, 2).Value = totalEgresos;

            worksheet.Cell(4, 1).Value = "Total Cuentas Por Cobrar";
            worksheet.Cell(4, 2).Value = totalCuentasPorCobrar;

            worksheet.Cell(5, 1).Value = "Total Cuentas Por Cobrar Vencidas";
            worksheet.Cell(5, 2).Value = totalCuentasPorCobrarVencidas;

            worksheet.Cell(6, 1).Value = "Cantidad Pendientes";
            worksheet.Cell(6, 2).Value = cantPendientes;

            worksheet.Cell(7, 1).Value = "Cantidad Pendientes Vencidas";
            worksheet.Cell(7, 2).Value = cantPendientesVencidas;

            // Formato monetario para las celdas con dinero
            for (int row = 2; row <= 5; row++)
            {
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "$#,##0.00";
            }

            // Ajustar ancho columnas
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            string fileName = $"ReporteFinanciero_{DateTime.Now:yyyyMMdd}.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

    }
}