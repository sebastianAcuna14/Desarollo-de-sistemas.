using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class ReparacionController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReparacionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Obtener las reparaciones creadas con el procedimiento almacenado y mostrarlas en la vista
        public IActionResult Index()
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var lista = context.Query<Reparacion>("ObtenerReparaciones").ToList();
                return View(lista);
            }
        }

        // ojo este get obtiene los datos 
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }



        //este post crea un nuevo producto, se usa el modelo de producto para obtener los datos del formulario
        [HttpPost]
        public IActionResult Crear(Reparacion reparacion)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.Execute("CrearReparacion",
                    new
                    {
                        reparacion.Fecha_Ingreso,
                        reparacion.Fecha_Salida,
                        reparacion.Descripcion,
                        reparacion.Estado,
                        reparacion.IdCliente
                    }
                    );
                if (resultado > 0)
                    return RedirectToAction("Index");
                return View(reparacion);

            }

        }

        //Ojo este get obtiene los datos de la reparacion que se va a editar
        [HttpGet]
        public IActionResult Editar(int id)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var reparacion = context.QueryFirstOrDefault<Reparacion>(
                    "SELECT * FROM Reparacion WHERE IdReparacion = @id", new { id });

                if (reparacion == null)
                    return NotFound();

                return View(reparacion);
            }
        }

        //OJO este post edita la reparacion y se usa el modelo de reparacion para obtener los datos del formulario
        [HttpPost]
        public IActionResult Editar(Reparacion reparacion)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var resultado = context.Execute("ActualizarReparacion",
                new
                {
                    reparacion.IdReparacion,
                    reparacion.Fecha_Ingreso,
                    reparacion.Fecha_Salida,
                    reparacion.Descripcion,
                    reparacion.Estado,
                    reparacion.IdCliente
                }
                );

                if (resultado > 0)
                    return RedirectToAction("Index");

                return View(reparacion);
            }
        }

        //Ojo este get obtiene los datos de la reparacion que se va a eliminar
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var reparacion = context.QueryFirstOrDefault<Reparacion>(
                    "SELECT * FROM Reparacion WHERE IdReparacion = @id", new { id });

                if (reparacion == null)
                    return NotFound();

                return View(reparacion);
            }
        }

        //Ojo este post elimina la reparacion y se usa el modelo de reparacion para obtener los datos del formulario
        [HttpPost]
        public IActionResult Eliminar(Reparacion reparacion)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var resultado = context.Execute("EliminarReparacion",
                    new { reparacion.IdReparacion },
                    commandType: CommandType.StoredProcedure);

                if (resultado > 0)
                    return RedirectToAction("Index");

                return View(reparacion); // por si algo falla, mantener vista
            }
        }

        //Ojo este get obtiene los datos de la reparacion que se va a consultar
        [HttpGet]
        public IActionResult Consultar(int id)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var reparacion = context.QueryFirstOrDefault<Reparacion>(
                    "SELECT * FROM Reparacion WHERE IdReparacion = @id", new { id });

                if (reparacion == null)
                    return NotFound();

                return View(reparacion);
            }
        }

        //Estas llaves hasta abajo son para editar un producto,
        //se usa el id del producto para buscarlo en la lista de productos y luego se muestra en la vista
        //[HttpGet]
        //public IActionResult Editar(int id)
        //{
        //    var Producto = Productos.FirstOrDefault(p => p.Id == id);
        //    if (Producto == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(Producto);
        //}

        //[HttpPost]
        //public IActionResult Editar(Producto producto)
        //{
        //    /*if (!ModelState.IsValid)
        //    {
        //        return View(producto);
        //    }*/

        //    var productoExistente = Productos.FirstOrDefault(p => p.Id == producto.Id);
        //    if (productoExistente == null)
        //    {
        //        return NotFound();
        //    }

        //    // Se actualizan los campos del producto existente
        //    productoExistente.Nombre_Producto = producto.Nombre_Producto;
        //    productoExistente.Categoria = producto.Categoria;
        //    productoExistente.Proveedor = producto.Proveedor;
        //    productoExistente.Precio = producto.Precio;
        //    productoExistente.Cantidad = producto.Cantidad;

        //    return RedirectToAction("Index");
        //}


        //    [HttpGet]
        //    public IActionResult Eliminar(int id)
        //    {
        //        var producto = Productos.FirstOrDefault(p => p.Id == id);
        //        if (producto == null)
        //            return NotFound();

        //        return View(producto);
        //    }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public IActionResult EliminarProducto(int id)
        //    {
        //        var producto = Productos.FirstOrDefault(p => p.Id == id);
        //        if (producto != null)
        //        {
        //            Productos.Remove(producto);
        //        }

        //        return RedirectToAction("Index");
        //    }


        //    [HttpGet]
        //    public IActionResult Consultar(int id)
        //    {
        //        var producto = Productos.FirstOrDefault(p => p.Id == id);
        //        if (producto == null)
        //            return NotFound();

        //        return View(producto);
        //    }
        //}
    }
}
