using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models;
using prototipo2.Servicios;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace prototipo2.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        private readonly IUtilitarios _utilitarios;

        public ClienteController(IConfiguration configuration, IUtilitarios utilitarios, IHostEnvironment environment)
        {
            _configuration = configuration;
            _utilitarios = utilitarios;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult InicioSesion()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RecuperarAcceso()
        {
            return View(); // muestra el formulario vacío
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
                    resultado.Token = _utilitarios.GenerarToken(resultado.idCliente);
                    return RedirectToAction("Index", "Home");

                }
                ViewBag.Mesaje = "No se pudo autenticar";
                return View();
            }

        }
        [HttpPost]

        [AllowAnonymous]
        public IActionResult RecuperarAcceso(Cliente cliente)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<Cliente>("ValidarCorreo",
                    new { cliente.Correo });

                if (resultado != null)
                {
                    var ContrasennaNotificar = _utilitarios.GenerarContrasenna(15);
                    var contrasena = _utilitarios.Encrypt(ContrasennaNotificar);

                    var resultadoActualizacion = context.Execute("ActualizarContrasenna",
                        new
                        {
                            resultado.idCliente,
                            contrasena
                        });

                    if (resultadoActualizacion > 0)
                    {
                        var ruta = Path.Combine(_environment.ContentRootPath, "Correos.html");
                        var html = System.IO.File.ReadAllText(ruta, UTF8Encoding.UTF8);

                        html = html.Replace("@@Usuario", resultado.Nombre);
                        html = html.Replace("@@contrasena", ContrasennaNotificar);

                        _utilitarios.EnviarCorreo(resultado.Correo!, "Recuperación de Acceso", html);
                        ViewBag.Mensaje = "Se han enviado las instrucciones de recuperación a su correo electrónico.";
                        return View();
                    }
                }

                return View();
            }
        }


    }
}
