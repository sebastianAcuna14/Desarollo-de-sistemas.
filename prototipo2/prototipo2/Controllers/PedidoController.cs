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



    }
    }

    