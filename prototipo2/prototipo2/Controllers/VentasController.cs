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
    public class VentasController : Controller
    {
        private readonly string _connectionString;

        public VentasController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Connection");
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<IActionResult> Index()
        {
            using var db = Connection;

            var ventas = await db.QueryAsync<Venta>("ObtenerVentas", commandType: CommandType.StoredProcedure);
            var ventasList = ventas.ToList();

            foreach (var venta in ventasList)
            {
                venta.Productos = (await db.QueryAsync<ItemVendido>("SELECT * FROM ItemsVendidos WHERE VentaId = @VentaId", new { VentaId = venta.Id })).ToList();
                venta.Pagos = (await db.QueryAsync<MetodoPago>("SELECT * FROM MetodosPago WHERE VentaId = @VentaId", new { VentaId = venta.Id })).ToList();
                venta.NotaCredito = await db.QueryFirstOrDefaultAsync<NotaCredito>("SELECT * FROM NotaCredito WHERE Id = @Id", new { Id = venta.NotaCreditoId });
                venta.Devoluciones = (await db.QueryAsync<Devolucion>("SELECT * FROM Devoluciones WHERE VentaId = @VentaId", new { VentaId = venta.Id })).ToList();

                foreach (var devolucion in venta.Devoluciones)
                {
                    devolucion.ProductosDevueltos = (await db.QueryAsync<ItemDevuelto>("SELECT * FROM ItemsDevueltos WHERE DevolucionId = @DevolucionId", new { DevolucionId = devolucion.Id })).ToList();
                }
            }

            return View(ventasList);
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
            if (venta.Productos == null || !venta.Productos.Any())
                ModelState.AddModelError("", "Debe agregar al menos un producto.");

            if (venta.Pagos == null || !venta.Pagos.Any())
                ModelState.AddModelError("", "Debe registrar al menos un método de pago.");

            if (venta.Pagos.GroupBy(p => p.Tipo).Any(g => g.Count() > 1))
                ModelState.AddModelError("", "No se pueden repetir métodos de pago del mismo tipo.");

            venta.Fecha = DateTime.Now;

            venta.Productos = venta.Productos
                .Where(p => !string.IsNullOrWhiteSpace(p.Producto) && p.Cantidad > 0 && p.PrecioUnitario > 0)
                .ToList();

            venta.Pagos = venta.Pagos
                .Where(p => !string.IsNullOrWhiteSpace(p.Tipo))
                .ToList();

            using var db = Connection;
            using var tx = db.BeginTransaction();

            try
            {
                var ventaId = await db.ExecuteScalarAsync<int>(
                    "CrearVenta",
                    new { Fecha = venta.Fecha, NotaCreditoId = (int?)null },
                    transaction: tx,
                    commandType: CommandType.StoredProcedure
                );

                foreach (var producto in venta.Productos)
                {
                    await db.ExecuteAsync(
                        @"INSERT INTO ItemsVendidos (VentaId, Producto, Cantidad, PrecioUnitario)
                  VALUES (@VentaId, @Producto, @Cantidad, @PrecioUnitario)",
                        new { VentaId = ventaId, producto.Producto, producto.Cantidad, producto.PrecioUnitario },
                        transaction: tx
                    );
                }

                foreach (var pago in venta.Pagos)
                {
                    await db.ExecuteAsync(
                        @"INSERT INTO MetodosPago (VentaId, Monto, Tipo)
                  VALUES (@VentaId, @Monto, @Tipo)",
                        new { VentaId = ventaId, pago.Monto, pago.Tipo },
                        transaction: tx
                    );
                }

                tx.Commit();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                tx.Rollback();
                ModelState.AddModelError("", "Error al registrar la venta.");
                return View(venta);
            }
        }



        public async Task<IActionResult> Details(int id)
        {
            using var db = Connection;
            var venta = await db.QueryFirstOrDefaultAsync<Venta>("SELECT * FROM Venta WHERE Id = @Id", new { Id = id });

            if (venta == null) return NotFound();

            venta.Productos = (await db.QueryAsync<ItemVendido>("SELECT * FROM ItemsVendidos WHERE VentaId = @VentaId", new { VentaId = id })).ToList();
            venta.Pagos = (await db.QueryAsync<MetodoPago>("SELECT * FROM MetodosPago WHERE VentaId = @VentaId", new { VentaId = id })).ToList();
            venta.NotaCredito = await db.QueryFirstOrDefaultAsync<NotaCredito>("SELECT * FROM NotaCredito WHERE Id = @Id", new { Id = venta.NotaCreditoId });
            venta.Devoluciones = (await db.QueryAsync<Devolucion>("SELECT * FROM Devoluciones WHERE VentaId = @VentaId", new { VentaId = id })).ToList();

            foreach (var devolucion in venta.Devoluciones)
            {
                devolucion.ProductosDevueltos = (await db.QueryAsync<ItemDevuelto>("SELECT * FROM ItemsDevueltos WHERE DevolucionId = @DevolucionId", new { DevolucionId = devolucion.Id })).ToList();
            }

            return View(venta);
        }

        [HttpGet]
        public async Task<IActionResult> AgregarDevolucion(int id)
        {
            using var db = Connection;
            var venta = await db.QueryFirstOrDefaultAsync<Venta>("SELECT * FROM Venta WHERE Id = @Id", new { Id = id });

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

            using var db = Connection;
            using var tx = db.BeginTransaction();

            try
            {
                // Insertar devolución
                var devolucionId = await db.ExecuteScalarAsync<int>(
                    @"INSERT INTO Devoluciones (VentaId, Fecha, Motivo)
              VALUES (@VentaId, @Fecha, @Motivo);
              SELECT SCOPE_IDENTITY();",
                    new { devolucion.VentaId, devolucion.Fecha, devolucion.Motivo },
                    transaction: tx
                );

                // Consultar venta y sus productos para calcular monto total
                var venta = await db.QueryFirstOrDefaultAsync<Venta>(
                    "SELECT * FROM Venta WHERE Id = @Id", new { Id = id }, tx);

                if (venta == null)
                {
                    tx.Rollback();
                    return NotFound();
                }

                var productos = (await db.QueryAsync<ItemVendido>(
                    "SELECT * FROM ItemsVendidos WHERE VentaId = @VentaId",
                    new { VentaId = id }, tx)).ToList();

                var montoTotal = productos.Sum(p => p.Cantidad * p.PrecioUnitario);

                if (venta.NotaCreditoId == null)
                {
                    // Crear nota de crédito
                    var notaCreditoId = await db.ExecuteScalarAsync<int>(
                        @"INSERT INTO NotaCredito (Fecha, Monto, Comentario)
                  VALUES (@Fecha, @Monto, @Comentario);
                  SELECT SCOPE_IDENTITY();",
                        new
                        {
                            Fecha = DateTime.Now,
                            Monto = montoTotal,
                            Comentario = $"Devolución registrada el {DateTime.Now:dd/MM/yyyy}"
                        },
                        transaction: tx
                    );

                    // Asociar nota de crédito a la venta
                    await db.ExecuteAsync(
                        "UPDATE Venta SET NotaCreditoId = @NotaCreditoId WHERE Id = @Id",
                        new { NotaCreditoId = notaCreditoId, Id = id },
                        transaction: tx
                    );
                }

                tx.Commit();
                return RedirectToAction("Details", new { id });
            }
            catch
            {
                tx.Rollback();
                ModelState.AddModelError("", "Error al registrar la devolución.");
                return View(devolucion);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = Connection;
            using var tx = db.BeginTransaction();

            try
            {
                // Obtener devoluciones con sus IDs
                var devoluciones = (await db.QueryAsync<Devolucion>(
                    "SELECT Id FROM Devoluciones WHERE VentaId = @VentaId",
                    new { VentaId = id }, tx)).ToList();

                foreach (var devolucion in devoluciones)
                {
                    await db.ExecuteAsync(
                        "DELETE FROM ItemsDevueltos WHERE DevolucionId = @DevolucionId",
                        new { DevolucionId = devolucion.Id }, tx);
                }

                await db.ExecuteAsync("DELETE FROM Devoluciones WHERE VentaId = @VentaId", new { VentaId = id }, tx);
                await db.ExecuteAsync("DELETE FROM ItemsVendidos WHERE VentaId = @VentaId", new { VentaId = id }, tx);
                await db.ExecuteAsync("DELETE FROM MetodosPago WHERE VentaId = @VentaId", new { VentaId = id }, tx);
                await db.ExecuteAsync("DELETE FROM Venta WHERE Id = @Id", new { Id = id }, tx);

                tx.Commit();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                tx.Rollback();
                return StatusCode(500, "Error al eliminar la venta.");
            }
        }
    }
}
