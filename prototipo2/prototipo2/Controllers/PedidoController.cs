using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class PedidoController : Controller
    {
        // Esta lista simula una base de datos en memoria para pedidos
        private static List<Pedido> Pedidos = new List<Pedido>
        {
            new() { Id = 1, Nombre_Producto = "Producto A", Numero_Pedido = "NP001", Cantidad = 10, FechaPedido = DateTime.Today.AddDays(-1), Precio = 150.00m, Estado = "En camino" },
            new() { Id = 2, Nombre_Producto = "Producto B", Numero_Pedido = "NP002", Cantidad = 5, FechaPedido = DateTime.Today, Precio = 250.00m, Estado = "Enviado" },
        };

        public IActionResult Index()
        {
            return View(Pedidos);
        }

        // Mostrar formulario para crear un pedido
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // Procesar creación del pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pedido pedido)
        {
            if (!ModelState.IsValid)
            {
                return View(pedido);
            }

            pedido.Id = Pedidos.Count + 1; // Asignar nuevo ID
            Pedidos.Add(pedido);
            return RedirectToAction("Index");
        }

        // Mostrar pedido para editar
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var pedido = Pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // Procesar edición del pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Pedido pedido)
        {
            if (!ModelState.IsValid)
            {
                return View(pedido);
            }

            var pedidoExistente = Pedidos.FirstOrDefault(p => p.Id == pedido.Id);
            if (pedidoExistente == null)
            {
                return NotFound();
            }

            pedidoExistente.Nombre_Producto = pedido.Nombre_Producto;
            pedidoExistente.Numero_Pedido = pedido.Numero_Pedido;
            pedidoExistente.Cantidad = pedido.Cantidad;
            pedidoExistente.FechaPedido = pedido.FechaPedido;
            pedidoExistente.Precio = pedido.Precio;
            pedidoExistente.Estado = pedido.Estado;

            return RedirectToAction("Index");
        }

        // Confirmar eliminación
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var pedido = Pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        // Eliminar pedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPedido(int id)
        {
            var pedido = Pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido != null)
            {
                Pedidos.Remove(pedido);
            }

            return RedirectToAction("Index");
        }

        // Consultar detalles de un pedido
        [HttpGet]
        public IActionResult Consultar(int id)
        {
            var pedido = Pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarComoEnCamino(int id)
        {
            var pedido = Pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido != null && pedido.Estado == "Preparando")
            {
                pedido.Estado = "En camino";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarComoEnviado(int id)
        {
            var pedido = Pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido != null && pedido.Estado == "En camino")
            {
                pedido.Estado = "Enviado";
            }
            return RedirectToAction("Index");
        }


    }
}
