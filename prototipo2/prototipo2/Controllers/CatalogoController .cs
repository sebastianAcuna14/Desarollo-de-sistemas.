using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using prototipo2.Models;
using System.Data;

namespace prototipo2.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly IConfiguration _configuration;

        public CatalogoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ✅ Mostrar todos los productos en el catálogo
        public IActionResult Index()
        {
            var connectionString = _configuration.GetConnectionString("Connection");

            using var connection = new SqlConnection(connectionString);

            var productos = connection.Query<Producto>(
                "ObtenerProductos", // Debes tener este SP igual que en Inventario
                commandType: CommandType.StoredProcedure
            ).ToList();

            return View(productos);
        }

        // ✅ Mostrar detalles de un producto en el catálogo
        public IActionResult Detalles(int id)
        {
            Producto producto;
            var connectionString = _configuration.GetConnectionString("Connection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    producto = connection.QueryFirstOrDefault<Producto>(
                        "ObtenerProductoPorId",
                        new { IdProducto = id },
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (producto == null)
                {
                    return NotFound();
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalles del producto: {ex.Message}");
                return View("Error");
            }
        }
    }
}