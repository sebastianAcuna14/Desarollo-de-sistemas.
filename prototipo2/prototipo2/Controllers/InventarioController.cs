using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models;
using System.Data;

namespace prototipo2.Controllers
{
    public class InventarioController : Controller
    {
        private readonly IConfiguration _configuration;

        public InventarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection Conexion()
        {
            return new SqlConnection(_configuration.GetConnectionString("Connection"));
        }

        // GET: Index
        public IActionResult Index()
        {
            using var con = Conexion();
            var productos = con.Query<Producto>("ObtenerProductos").ToList();
            return View(productos);
        }

        // GET: Crear
        [HttpGet]
        public IActionResult Crear()
        {
            CargarCategoriasYProveedores();
            return View();
        }

        // POST: Crear
        [HttpPost]
        public IActionResult Crear(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                CargarCategoriasYProveedores();
                return View(producto);
            }

            using var con = Conexion();
            con.Execute("CrearProducto", new
            {
                producto.Nombre,
                producto.Descripcion,
                producto.Cantidad,
                producto.Precio,
                producto.IdProveedor,
                producto.IdCategoria
            });


            return RedirectToAction("Index");
        }

        // GET: Editar
        [HttpGet]
        public IActionResult Editar(int id)
        {
            using var con = Conexion();
            var producto = con.QueryFirstOrDefault<Producto>("ObtenerProductoPorId", new { IdProducto = id });

            if (producto == null)
                return NotFound();

            CargarCategoriasYProveedores();
            return View(producto);
        }

        // POST: Editar
        [HttpPost]
        public IActionResult Editar(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                CargarCategoriasYProveedores();
                return View(producto);
            }

            using var con = Conexion();
            con.Execute("ActualizarProducto", new
            {
                producto.IdProducto,
                producto.Nombre,
                producto.Descripcion,
                producto.Cantidad,
                producto.Categoria,
                producto.Precio,
                producto.IdProveedor,
                producto.IdCategoria
            });

            return RedirectToAction("Index");
        }

        // GET: Eliminar
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            using var con = Conexion();
            var producto = con.QueryFirstOrDefault<Producto>("ObtenerProductoPorId", new { IdProducto = id });

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // POST: Eliminar
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            using var con = Conexion();
            con.Execute("EliminarProducto", new { IdProducto = id });
            return RedirectToAction("Index");
        }

        // para dropdowns de categoria y proveedores
        private void CargarCategoriasYProveedores()
        {
            using var con = Conexion();

            var listaProveedores = con.Query<Proveedor>("ObtenerProveedores")
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = p.IDProveedor.ToString(),
                    Text = p.NombreEmpresa
                }).ToList();

            var listaCategorias = con.Query<Categoria>("ObtenerCategorias")
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.Nombre
                }).ToList();

            ViewBag.ListaProveedores = listaProveedores;
            ViewBag.ListaCategorias = listaCategorias;
        }
    }
}
