using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using prototipo2.Models;
using prototipo2.Services;
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
        private readonly PayPalService _payPalService;

        public CarritoController(IConfiguration config, PayPalService payPalService)
        {
            _connectionString = config.GetConnectionString("Connection");
            _payPalService = payPalService;
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
            var carrito = (await Connection.QueryAsync<CarritoDTO>(
                "ObtenerCarrito",
                commandType: CommandType.StoredProcedure
            )).ToList();

            ViewBag.TotalPagar = carrito.Sum(x => x.Subtotal);
            return View(carrito);
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
        public async Task<IActionResult> ActualizarCantidad(int id, string action)
        {
            using var db = Connection;

            var item = await db.QueryFirstOrDefaultAsync<CarritoDTO>(
                "SELECT * FROM Carrito WHERE Id = @Id",
                new { Id = id }
            );

            if (item == null)
                return RedirectToAction(nameof(Index));

            int nuevaCantidad = item.Cantidad;

            if (action == "increase")
                nuevaCantidad++;
            else if (action == "decrease")
                nuevaCantidad--;

            if (nuevaCantidad <= 0)
            {
                await db.ExecuteAsync("DELETE FROM Carrito WHERE Id = @Id", new { Id = id });
            }
            else
            {
                await db.ExecuteAsync(
                    "ActualizarCantidadCarrito",
                    new { Id = id, Cantidad = nuevaCantidad },
                    commandType: CommandType.StoredProcedure
                );
            }

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

        // Método para confirmar pago usando PayPalService para validar orden
        [HttpPost]
        public async Task<IActionResult> ConfirmarPago([FromBody] ConfirmarPagoDTO model)
        {
            using var db = Connection;
            db.Open();
            using var transaction = db.BeginTransaction();

            try
            {
                // Validar orden PayPal
                bool isPagoValido = await _payPalService.ValidarOrdenAsync(model.orderId);
                if (!isPagoValido)
                {
                    return Json(new { success = false, error = "Pago no confirmado por PayPal." });
                }

                // Obtener carrito para registrar la venta
                var carrito = (await db.QueryAsync<CarritoDTO>("ObtenerCarrito", transaction: transaction, commandType: CommandType.StoredProcedure)).ToList();
                if (!carrito.Any())
                {
                    return Json(new { success = false, error = "El carrito está vacío." });
                }

                // Insertar venta con datos del pago y contacto
                var fechaVenta = DateTime.Now;
                var ventaId = await db.ExecuteScalarAsync<int>(
                    @"INSERT INTO Venta (Fecha, NotaCreditoId, MetodoPago, MontoTotal, Contacto, Telefono, Direccion, ProvinciaId, DepartamentoId, DistritoId, PaypalOrderId)
                      VALUES (@Fecha, NULL, 'Paypal', @MontoTotal, @Contacto, @Telefono, @Direccion, @Provincia, @Departamento, @Distrito, @OrderId);
                      SELECT CAST(SCOPE_IDENTITY() as int);",
                    new
                    {
                        Fecha = fechaVenta,
                        MontoTotal = model.total,
                        Contacto = model.contacto,
                        Telefono = model.telefono,
                        Direccion = model.direccion,
                        Provincia = model.IdProvincia,
                        Departamento = model.IdDepartamento,
                        Distrito = model.IdDistrito,
                        OrderId = model.orderId
                    },
                    transaction
                );

                // Insertar cada producto vendido
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

                // Limpiar carrito después de la venta
                await db.ExecuteAsync("LimpiarCarrito", transaction: transaction, commandType: CommandType.StoredProcedure);

                transaction.Commit();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Vista de agradecimiento tras pago exitoso
        public IActionResult Gracias()
        {
            return View("ProcesarPago"); 
        }
    }
}
