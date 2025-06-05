using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class InventarioController : Controller
    {
        // esta lista simula una base de datos en memoria para los productos
        private static List<Producto> Productos = new List<Producto>
        {
            new() { Id = 1, Nombre_Producto = "Producto A", Categoria = "Categoria 1", Cantidad = 10, Proveedor = "Proveedor X", Precio = 100.00m },
            new() { Id = 2, Nombre_Producto = "Producto B", Categoria = "Categoria 2", Cantidad = 20, Proveedor = "Proveedor Y", Precio = 200.00m },
        };

        public IActionResult Index()
        {

            return View(Productos);
        }


        //esto es para crear un producto, pero no lo guarda en la base de datos, solo lo muestra en la vista
        public IActionResult Crear()
        {
            return View();
        }


        //esto crea un producto temporalmente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Producto producto)
        {
            /*if (ModelState.IsValid)
            {
                
            }*/
            producto.Id = Productos.Count + 1; // Asignar un nuevo ID
            Productos.Add(producto);//esto es para trabajar en memoria, en una base de datos se haría diferente
            return RedirectToAction("Index");

        }


        //Estas llaves hasta abajo son para editar un producto,
        //se usa el id del producto para buscarlo en la lista de productos y luego se muestra en la vista
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var Producto = Productos.FirstOrDefault(p => p.Id == id);
            if (Producto == null)
            {
                return NotFound();
            }

            return View(Producto);
        }

        [HttpPost]
        public IActionResult Editar(Producto producto)
        {
            /*if (!ModelState.IsValid)
            {
                return View(producto);
            }*/

            var productoExistente = Productos.FirstOrDefault(p => p.Id == producto.Id);
            if (productoExistente == null)
            {
                return NotFound();
            }

            // Se actualizan los campos del producto existente
            productoExistente.Nombre_Producto = producto.Nombre_Producto;
            productoExistente.Categoria = producto.Categoria;
            productoExistente.Proveedor = producto.Proveedor;
            productoExistente.Precio = producto.Precio;
            productoExistente.Cantidad = producto.Cantidad;

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var producto = Productos.FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarProducto(int id)
        {
            var producto = Productos.FirstOrDefault(p => p.Id == id);
            if (producto != null)
            {
                Productos.Remove(producto);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Consultar(int id)
        {
            var producto = Productos.FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }
    }
}