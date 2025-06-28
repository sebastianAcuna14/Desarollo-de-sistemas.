using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly IConfiguration _configuration;

        public CategoriaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection Conexion()
        {
            return new SqlConnection(_configuration.GetConnectionString("Connection"));
        }

        public IActionResult Index()
        {
            using var context = Conexion();
            var categorias = context.Query<Categoria>("ObtenerCategorias").ToList();
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Categoria categoria)
        {
            using var con = Conexion();

            var result = con.Execute("CrearCategoria",
                new { categoria.Nombre, categoria.Descripcion });

            if (result > 0)
                return RedirectToAction("Index");

            return View();
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            using var con = Conexion();

            var categoria = con.QueryFirstOrDefault<Categoria>(
                "ConsultarCategoriaID", new { IdCategoria = id });

            return View(categoria);
        }
        [HttpPost]
        public IActionResult Editar(Categoria categoria)
        {
            using var con = Conexion();

            var result = con.Execute("ActualizarCategoria", new
            {
                categoria.IdCategoria,
                categoria.Nombre,
                categoria.Descripcion
            });

            if (result > 0)
                return RedirectToAction("Index");

            return View(categoria);
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            using var con = Conexion();

            var categoria = con.QueryFirstOrDefault<Categoria>(
                "ConsultarCategoriaID", new { IdCategoria = id });

            return View(categoria);
        }

        [HttpPost]
        public IActionResult Eliminar(Categoria categoria)
        {
            using var con = Conexion();

            var result = con.Execute("EliminarCategoria", new { categoria.IdCategoria });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Consultar(int id)
        {
            using var con = Conexion();

            var categoria = con.QueryFirstOrDefault<Categoria>(
                "ConsultarCategoriaID", new { IdCategoria = id });

            return View(categoria);
        }


    }




}

