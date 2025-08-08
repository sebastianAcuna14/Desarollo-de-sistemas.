using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using prototipo2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace prototipo2.Controllers
{
    public class InventarioController : Controller
    {
        private readonly IConfiguration _configuration;

        public InventarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection Conexion() => new SqlConnection(_configuration.GetConnectionString("Connection"));

        // GET: Index
        public IActionResult Index()
        {
            using var con = Conexion();
            var productos = con.Query<Producto>(
                "ObtenerProductos",
                commandType: CommandType.StoredProcedure
            ).ToList();
            return View(productos);
        }

        // GET: Crear
        [HttpGet]
        public IActionResult Crear()
        {
            using var con = Conexion();
            var proveedores = con.Query<Proveedor>(
                "ObtenerProveedores",
                commandType: CommandType.StoredProcedure
            ).ToList();
            ViewBag.ListaProveedores = new SelectList(proveedores, "IDProveedor", "NombreEmpresa");

            var categorias = con.Query<Categoria>(
                "ObtenerInventario",
                commandType: CommandType.StoredProcedure
            ).ToList();
            ViewBag.ListaCategorias = new SelectList(categorias, "IdCategoria", "Nombre");

            return View();
        }

        // POST: Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Producto producto)
        {
            using var con = Conexion();

            if (!ModelState.IsValid)
            {
                var proveedores = con.Query<Proveedor>(
                    "ObtenerProveedores",
                    commandType: CommandType.StoredProcedure
                ).ToList();
                ViewBag.ListaProveedores = new SelectList(proveedores, "IDProveedor", "NombreEmpresa");

                var categorias = con.Query<Categoria>(
                    "ObtenerCategorias",
                    commandType: CommandType.StoredProcedure
                ).ToList();
                ViewBag.ListaCategorias = new SelectList(categorias, "IdCategoria", "Nombre");

                return View(producto);
            }

            con.Execute(
                "CrearProducto",
                new
                {
                    producto.Nombre,
                    producto.Descripcion,
                    producto.Cantidad,
                    producto.Precio,
                    producto.IdProveedor,
                    producto.IdCategoria
                },
                commandType: CommandType.StoredProcedure
            );

            return RedirectToAction(nameof(Index));
        }

        // GET: Editar
        [HttpGet]
        public IActionResult Editar(int id)
        {
            using var con = Conexion();
            var producto = con.QueryFirstOrDefault<Producto>(
                "ObtenerProductoPorId",
                new { IdProducto = id },
                commandType: CommandType.StoredProcedure
            );

            if (producto == null)
                return NotFound();

            var proveedores = con.Query<Proveedor>(
                "ObtenerProveedores",
                commandType: CommandType.StoredProcedure
            ).ToList();
            ViewBag.ListaProveedores = new SelectList(proveedores, "IDProveedor", "NombreEmpresa");

            var categorias = con.Query<Categoria>(
                "ObtenerCategorias",
                commandType: CommandType.StoredProcedure
            ).ToList();
            ViewBag.ListaCategorias = new SelectList(categorias, "IdCategoria", "Nombre");

            return View(producto);
        }

        // POST: Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Producto producto)
        {
            using var con = Conexion();

            if (!ModelState.IsValid)
            {
                var proveedores = con.Query<Proveedor>(
                    "ObtenerProveedores",
                    commandType: CommandType.StoredProcedure
                ).ToList();
                ViewBag.ListaProveedores = new SelectList(proveedores, "IDProveedor", "NombreEmpresa");

                var categorias = con.Query<Categoria>(
                    "ObtenerCategorias",
                    commandType: CommandType.StoredProcedure
                ).ToList();
                ViewBag.ListaCategorias = new SelectList(categorias, "IdCategoria", "Nombre");

                return View(producto);
            }

            con.Execute(
                "ActualizarProducto",
                new
                {
                    producto.IdProducto,
                    producto.Nombre,
                    producto.Descripcion,
                    producto.Cantidad,
                    producto.Precio,
                    producto.IdProveedor,
                    producto.IdCategoria
                },
                commandType: CommandType.StoredProcedure
            );

            return RedirectToAction(nameof(Index));
        }

        // GET: Eliminar
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            using var con = Conexion();
            var producto = con.QueryFirstOrDefault<Producto>(
                "ObtenerProductoPorId",
                new { IdProducto = id },
                commandType: CommandType.StoredProcedure
            );

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // POST: Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            using var con = Conexion();
            con.Execute(
                "EliminarProducto",
                new { IdProducto = id },
                commandType: CommandType.StoredProcedure
            );
            return RedirectToAction(nameof(Index));
        }
    }
}
