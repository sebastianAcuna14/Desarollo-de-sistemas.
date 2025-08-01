using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models;
using System.Data;

namespace prototipo2.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProveedorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection Conexion()
        {
            return new SqlConnection(_configuration.GetConnectionString("Connection"));
        }

        // GET: Proveedor
        public IActionResult Index()
        {
            using var con = Conexion();
            var proveedores = con.Query<Proveedor>("ObtenerProveedores").ToList();
            return View(proveedores);

        }

        // GET: Crear
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Crear
        [HttpPost]
        public IActionResult Crear(Proveedor proveedor)
        {
            if (!ModelState.IsValid)
                return View(proveedor);

            using var con = Conexion();
            con.Execute("CrearProveedor", new
            {
                proveedor.NombreEmpresa,
                proveedor.Correo,
                proveedor.Telefono,
                proveedor.Estado
            });

            return RedirectToAction("Index");
        }

        // GET: Editar
        [HttpGet]
        public IActionResult Editar(int id)
        {
            using var con = Conexion();
            var proveedor = con.QueryFirstOrDefault<Proveedor>(
                "ObtenerProveedorPorId",
                new { IDProveedor = id });

            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }

        // POST: Editar
        [HttpPost]
        public IActionResult Editar(Proveedor proveedor)
        {
            if (!ModelState.IsValid)
                return View(proveedor);

            using var con = Conexion();
            con.Execute("ActualizarProveedor", new
            {
                proveedor.IDProveedor,
                proveedor.NombreEmpresa,
                proveedor.Correo,
                proveedor.Telefono,
                proveedor.Estado
            });

            return RedirectToAction("Index");
        }

        // GET: Consultar
        [HttpGet]
        public IActionResult Consultar(int id)
        {
            using var con = Conexion();
            var proveedor = con.QueryFirstOrDefault<Proveedor>(
                "ObtenerProveedorPorId",
                new { IDProveedor = id });

            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }

        // GET: Eliminar
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            using var con = Conexion();
            var proveedor = con.QueryFirstOrDefault<Proveedor>(
                "ObtenerProveedorPorId",
                new { IDProveedor = id });

            if (proveedor == null)
                return NotFound();

            return View(proveedor);
        }

        // POST: Eliminar
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            using var con = Conexion();
            con.Execute("EliminarProveedor", new { IDProveedor = id });
            return RedirectToAction("Index");
        }
    }
}
