using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IConfiguration _configuration;

        public ClienteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult InicioSesion()
        {
            return View();
        }



        [HttpGet]
        public IActionResult RegistroCliente()
        {
            return View();
        }


        [HttpPost]
        public IActionResult RegistroCliente(Cliente cliente)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.Execute("RegistrarCliente",
                      new
                      {
                          cliente.Nombre,
                          cliente.Apellido,
                          cliente.Cedula,
                          cliente.Correo,
                          cliente.Telefonos,
                          cliente.Contrasena
                      }

                      );
                if (resultado > 0)
                {
                    return RedirectToAction("Index", "Home");
                }


            }
            ViewBag.Mesaje = "No se pudo registrar";
            return View();

        }



        [HttpPost]
        public IActionResult InicioSesion(Cliente cliente)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<Cliente>("ValidarInicioSesion",
                    new
                    {
                        cliente.Correo,
                        cliente.Contrasena
                    });
                if (resultado != null)
                {
                    return RedirectToAction("Index", "Home");

                }
                ViewBag.Mesaje = "No se pudo autenticar";
                return View();
            }

        }


    }
}
