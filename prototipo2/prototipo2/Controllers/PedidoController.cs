using ClosedXML.Excel;
using System.IO;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IConfiguration _configuration;

        public PedidoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Mostrar todos los pedidos usando procedimiento almacenado
        public IActionResult Index()
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var lista = context.Query<Pedido>("ObtenerPedido", commandType: CommandType.StoredProcedure).ToList();
                return View(lista);
            }
        }

        // Mostrar formulario para crear un pedido
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // Procesar creación de un pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pedido pedido)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo es inválido, regresa a la vista con los errores
                return View(pedido);
            }

            Console.WriteLine($"Precio recibido: {pedido.Precio}");

            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var resultado = context.Execute("Crear_Pedido", new
                {
                    pedido.Nombre_Producto,
                    pedido.Numero_Pedido,
                    pedido.Cantidad,
                    pedido.FechaPedido,
                    pedido.Precio,
                    pedido.Estado
                }, commandType: CommandType.StoredProcedure);

                if (resultado > 0)
                    return RedirectToAction("Index");

                return View(pedido);
            }
        }



        // Mostrar pedido para editar
        [HttpGet]
        public IActionResult Editar(int id)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var pedido = context.QueryFirstOrDefault<Pedido>(
                    "SELECT * FROM Pedido WHERE Id = @id", new { id });

                if (pedido == null)
                    return NotFound();

                return View(pedido);
            }
        }

        // Procesar edición del pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Pedido pedido)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var resultado = context.Execute("ActualizarPedido", new
                {
                    pedido.Id,
                    pedido.Nombre_Producto,
                    pedido.Numero_Pedido,
                    pedido.Cantidad,
                    pedido.FechaPedido,
                    pedido.Precio,
                    pedido.Estado
                }, commandType: CommandType.StoredProcedure);

                if (resultado > 0)
                    return RedirectToAction("Index");

                return View(pedido);
            }
        }

        // Mostrar pedido para confirmar eliminación
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var pedido = context.QueryFirstOrDefault<Pedido>(
                    "SELECT * FROM Pedido WHERE Id = @id", new { id });

                if (pedido == null)
                    return NotFound();

                return View(pedido);
            }
        }

        // Procesar eliminación del pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Pedido pedido)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var resultado = context.Execute("EliminarPedido", new { pedido.Id }, commandType: CommandType.StoredProcedure);

                if (resultado > 0)
                    return RedirectToAction("Index");

                return View(pedido);
            }
        }

        // Consultar detalles de un pedido
        [HttpGet]
        public IActionResult Consultar(int id)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var pedido = context.QueryFirstOrDefault<Pedido>(
                    "SELECT * FROM Pedido WHERE Id = @id", new { id });

                if (pedido == null)
                    return NotFound();

                return View(pedido);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarComoEnCamino(int id)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                context.Execute("MarcarPedidoComoEnCamino", new { Id = id }, commandType: CommandType.StoredProcedure);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarComoEnviado(int id)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                context.Execute("MarcarPedidoComoEnviado", new { Id = id }, commandType: CommandType.StoredProcedure);
            }

            return RedirectToAction("Index");
        }

        public IActionResult ExportarExcel()
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var pedidos = context.Query<Pedido>("ObtenerPedido", commandType: CommandType.StoredProcedure).ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Pedidos");

                    // Encabezados
                    worksheet.Cell(1, 1).Value = "Número de Pedido";
                    worksheet.Cell(1, 2).Value = "Nombre del Producto";
                    worksheet.Cell(1, 3).Value = "Cantidad";
                    worksheet.Cell(1, 4).Value = "Precio";
                    worksheet.Cell(1, 5).Value = "Fecha del Pedido";
                    worksheet.Cell(1, 6).Value = "Estado";

                    int fila = 2;

                    foreach (var pedido in pedidos)
                    {
                        worksheet.Cell(fila, 1).Value = pedido.Numero_Pedido;
                        worksheet.Cell(fila, 2).Value = pedido.Nombre_Producto;
                        worksheet.Cell(fila, 3).Value = pedido.Cantidad;
                        worksheet.Cell(fila, 4).Value = pedido.Precio;
                        worksheet.Cell(fila, 5).Value = pedido.FechaPedido.ToShortDateString();
                        worksheet.Cell(fila, 6).Value = pedido.Estado;
                        fila++;
                    }

                    // Opcional: ajustar tamaño de las columnas
                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "ListadoPedidos.xlsx");
                    }


                }
            }
        }
    }
}