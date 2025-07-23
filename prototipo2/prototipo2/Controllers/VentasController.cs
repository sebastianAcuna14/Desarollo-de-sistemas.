using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using prototipo2.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

        // Listar todas las ventas con sus datos relacionados
        public async Task<IActionResult> Index()
        {
            using var db = Connection;
            var ventas = (await db.QueryAsync<Venta>("SELECT * FROM Venta ORDER BY Fecha DESC")).ToList();

            foreach (var venta in ventas)
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

            return View(ventas);
        }

        // Mostrar formulario para crear venta
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

            venta.Fecha = DateTime.Now;

            // Filtrar productos válidos
            venta.Productos = venta.Productos
                .Where(p => !string.IsNullOrWhiteSpace(p.Producto) && p.Cantidad > 0 && p.PrecioUnitario > 0)
                .ToList();

            // Filtrar pagos válidos
            venta.Pagos = venta.Pagos
                .Where(p => !string.IsNullOrWhiteSpace(p.Tipo))
                .ToList();

            using var db = Connection;
            db.Open();
            using var transaction = db.BeginTransaction();

            try
            {

                var ventaId = await db.ExecuteScalarAsync<int>(
                    @"INSERT INTO Venta (Fecha, NotaCreditoId) 
              VALUES (@Fecha, @NotaCreditoId);
              SELECT CAST(SCOPE_IDENTITY() as int);",
                    new { Fecha = venta.Fecha, NotaCreditoId = (int?)null },
                    transaction
                );


                foreach (var producto in venta.Productos)
                {
                    await db.ExecuteAsync(
                        @"INSERT INTO ItemsVendidos (VentaId, Producto, Cantidad, PrecioUnitario)
                  VALUES (@VentaId, @Producto, @Cantidad, @PrecioUnitario)",
                        new { VentaId = ventaId, producto.Producto, producto.Cantidad, producto.PrecioUnitario },
                        transaction
                    );
                }

                // Insertar métodos de pago (Monto = suma de productos si no se especifica)
                decimal totalVenta = venta.Productos.Sum(p => p.Total);
                foreach (var pago in venta.Pagos)
                {
                    var monto = pago.Monto > 0 ? pago.Monto : totalVenta; // fallback
                    await db.ExecuteAsync(
                        @"INSERT INTO MetodosPago (VentaId, Monto, Tipo)
                  VALUES (@VentaId, @Monto, @Tipo)",
                        new { VentaId = ventaId, Monto = monto, pago.Tipo },
                        transaction
                    );
                }

                transaction.Commit();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ModelState.AddModelError("", "Error al registrar la venta: " + ex.Message);
                return View(venta);
            }
        }



        // Mostrar detalles de una venta con todo lo relacionado
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

        // Eliminar venta con sus dependencias
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = Connection;

            // Eliminar productos devueltos, devoluciones, productos vendidos, pagos y venta (en ese orden)

            var devoluciones = (await db.QueryAsync<Devolucion>("SELECT * FROM Devoluciones WHERE VentaId = @VentaId", new { VentaId = id })).ToList();

            foreach (var devolucion in devoluciones)
            {
                await db.ExecuteAsync("DELETE FROM ItemsDevueltos WHERE DevolucionId = @DevolucionId", new { DevolucionId = devolucion.Id });
            }

            await db.ExecuteAsync("DELETE FROM Devoluciones WHERE VentaId = @VentaId", new { VentaId = id });
            await db.ExecuteAsync("DELETE FROM ItemsVendidos WHERE VentaId = @VentaId", new { VentaId = id });
            await db.ExecuteAsync("DELETE FROM MetodosPago WHERE VentaId = @VentaId", new { VentaId = id });
            await db.ExecuteAsync("DELETE FROM Venta WHERE Id = @Id", new { Id = id });

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AgregarDevolucion(int id)
        {
            using var db = Connection;
            var venta = await db.QueryFirstOrDefaultAsync<Venta>(
                "SELECT * FROM Venta WHERE Id = @Id", new { Id = id });

            if (venta == null)
                return NotFound();

            var devolucion = new Devolucion
            {
                VentaId = venta.Id,
                Fecha = DateTime.Now
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

            devolucion.Fecha = DateTime.Now;
            devolucion.VentaId = id;

            using var db = Connection;
            db.Open();
            using var tx = db.BeginTransaction();

            try
            {
                // Insertar la devolución
                var devolucionId = await db.ExecuteScalarAsync<int>(
                    @"INSERT INTO Devoluciones (VentaId, Fecha, Motivo)
              VALUES (@VentaId, @Fecha, @Motivo);
              SELECT CAST(SCOPE_IDENTITY() as int);",
                    new { devolucion.VentaId, devolucion.Fecha, devolucion.Motivo },
                    transaction: tx
                );

                // Obtener productos de la venta
                var productos = (await db.QueryAsync<ItemVendido>(
                    "SELECT * FROM ItemsVendidos WHERE VentaId = @VentaId",
                    new { VentaId = id }, tx)).ToList();

                if (!productos.Any())
                {
                    tx.Rollback();
                    ModelState.AddModelError("", "No hay productos para devolver.");
                    return View(devolucion);
                }

                var montoTotal = productos.Sum(p => p.Cantidad * p.PrecioUnitario);

                // Crear Nota de Crédito si no existe
                var notaCreditoId = await db.ExecuteScalarAsync<int>(
                    "SELECT NotaCreditoId FROM Venta WHERE Id = @Id",
                    new { Id = id }, tx
                );

                if (notaCreditoId == 0)
                {
                    notaCreditoId = await db.ExecuteScalarAsync<int>(
                        @"INSERT INTO NotaCredito (Fecha, Monto, Comentario)
                  VALUES (@Fecha, @Monto, @Comentario);
                  SELECT CAST(SCOPE_IDENTITY() as int);",
                        new
                        {
                            Fecha = DateTime.Now,
                            Monto = montoTotal,
                            Comentario = $"Devolución registrada el {DateTime.Now:dd/MM/yyyy}"
                        },
                        transaction: tx
                    );

                    await db.ExecuteAsync(
                        "UPDATE Venta SET NotaCreditoId = @NotaCreditoId WHERE Id = @Id",
                        new { NotaCreditoId = notaCreditoId, Id = id },
                        transaction: tx
                    );
                }

                tx.Commit();
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                tx.Rollback();
                ModelState.AddModelError("", "Error al registrar la devolución: " + ex.Message);
                return View(devolucion);
            }
        }

      

    }
}