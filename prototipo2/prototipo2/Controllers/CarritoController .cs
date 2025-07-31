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
    public class CarritoController : Controller
    {
        private readonly string _connectionString;

        public CarritoController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Connection");
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

 
        public async Task<IActionResult> Index()
        {
            using var db = Connection;
            var carrito = (await db.QueryAsync<CarritoDTO>(
                "ObtenerCarrito",
                commandType: CommandType.StoredProcedure
            )).ToList();

            return View(carrito);
        }

        public async Task<IActionResult> Paypal()
        {
            var carrito = await Connection.QueryAsync<CarritoDTO>("ObtenerCarrito", commandType: CommandType.StoredProcedure);
            return View(carrito.ToList());
        }


[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(CarritoDTO carrito)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index)); 

            using var db = Connection;
            var parameters = new DynamicParameters();
            parameters.Add("@ProductoId", carrito.ProductoId);
            parameters.Add("@Cantidad", carrito.Cantidad);
            parameters.Add("@Nombre_Producto", carrito.Nombre_Producto);
            parameters.Add("@Precio", carrito.Precio);

            await db.ExecuteAsync(
                "AgregarAlCarrito",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarCantidad(int id, int cantidad)
        {
            using var db = Connection;

            await db.ExecuteAsync(
                "ActualizarCantidadCarrito",
                new { Id = id, Cantidad = cantidad },
                commandType: CommandType.StoredProcedure
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var db = Connection;

            await db.ExecuteAsync(
                "EliminarDelCarrito",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vaciar()
        {
            using var db = Connection;

            await db.ExecuteAsync(
                "LimpiarCarrito",
                commandType: CommandType.StoredProcedure
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcesarPago(string correoCliente = "cliente@email.com")
        {
            using var db = Connection;
            db.Open();
            using var transaction = db.BeginTransaction();

            try
            {
                // 1. Obtener productos del carrito
                var carrito = (await db.QueryAsync<CarritoDTO>(
                    "ObtenerCarrito", transaction: transaction, commandType: CommandType.StoredProcedure)).ToList();

                if (!carrito.Any())
                {
                    ModelState.AddModelError("", "El carrito está vacío.");
                    return RedirectToAction(nameof(Index));
                }

                // 2. Insertar venta
                var fechaVenta = DateTime.Now;
                var ventaId = await db.ExecuteScalarAsync<int>(
                    @"INSERT INTO Venta (Fecha, NotaCreditoId) 
              VALUES (@Fecha, NULL);
              SELECT CAST(SCOPE_IDENTITY() as int);",
                    new { Fecha = fechaVenta },
                    transaction
                );

                // 3. Insertar productos vendidos
                foreach (var item in carrito)
                {
                    await db.ExecuteAsync(
                        @"INSERT INTO ItemsVendidos (VentaId, Producto, Cantidad, PrecioUnitario)
                  VALUES (@VentaId, @Producto, @Cantidad, @PrecioUnitario)",
                        new
                        {
                            VentaId = ventaId,
                            Producto = item.Nombre_Producto,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = item.Precio
                        },
                        transaction
                    );
                }

                // 4. Insertar método de pago
                decimal totalVenta = carrito.Sum(p => p.Precio * p.Cantidad);
                await db.ExecuteAsync(
                    @"INSERT INTO MetodosPago (VentaId, Monto, Tipo)
              VALUES (@VentaId, @Monto, @Tipo)",
                    new { VentaId = ventaId, Monto = totalVenta, Tipo = "Paypal" },
                    transaction
                );

                // 5. Limpiar el carrito
                await db.ExecuteAsync("LimpiarCarrito", transaction: transaction, commandType: CommandType.StoredProcedure);

                transaction.Commit();

                return RedirectToAction("Confirmacion");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ModelState.AddModelError("", "Error al procesar el pago: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }



        public IActionResult Confirmacion()
        {
            return View("ProcesarPago");
        }

    }
}
