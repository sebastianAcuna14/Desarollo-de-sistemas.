using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models; 
using System.Linq;
namespace prototipo2.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly IConfiguration _configuration;
        public EmpleadoController(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        // Acción Index que envía la lista a la vista


        public IActionResult CrearEmpleado()
        {
            return View();
        }
        public IActionResult ListaEmpleado()
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.Query<Empleado>("ObtenerEmpleado").ToList();
                return View(resultado);
            }
        }
        
        //[HttpGet]
        //public IActionResult EditarEmpleado(int id)
        //{
        //    var empleado = Empleados.FirstOrDefault(e => e.Id == id);
        //    if (empleado == null) return NotFound();
        //    return View(empleado);


        //}
        [HttpPost]
        public IActionResult CrearEmpleado(Empleado empleado)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.Execute("RegistrarEmpleado",
                      new
                      {
                          empleado.Nombre,
                          empleado.Apellido,
                          empleado.Correo,
                          empleado.Telefono,
                          empleado.Contrasena,
                         
                      }

                      );
                if (resultado > 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(empleado);
            }
        }


        //public IActionResult EliminarEmpleado(int id)
        //{
        //    var empleado = Empleados.FirstOrDefault(e => e.Id == id);
        //    if (empleado == null) return NotFound();
        //    return View(empleado);
        //}

        //[HttpPost]
        //public JsonResult AgregarDesdeModal([FromBody] Empleado nuevoEmpleado)
        //{
        //    int nuevoId = Empleados.Any() ? Empleados.Max(e => e.Id) + 1 : 1;
        //    nuevoEmpleado.Id = nuevoId;
        //    Empleados.Add(nuevoEmpleado);

        //    return Json(new { success = true });
        //}




        //[HttpPost]
        //public IActionResult EditarEmpleado(int id, Empleado empleado)
        //{
        //    if (id != empleado.Id) return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        var existente = Empleados.FirstOrDefault(e => e.Id == id);
        //        if (existente == null) return NotFound();

        //        existente.NombreCompleto = empleado.NombreCompleto;
        //        existente.Cedula = empleado.Cedula;
        //        existente.Rol = empleado.Rol;
        //        existente.Salario = empleado.Salario;
        //        existente.FechaContratacion = empleado.FechaContratacion;

        //        return RedirectToAction(nameof(ListaEmpleado));
        //    }
        //    return View(empleado);
        //}
        

        //public IActionResult Eliminar(int id)
        //{
        //    var empleado = Empleados.FirstOrDefault(e => e.Id == id);
        //    if (empleado !=null)
        //    {
        //        Empleados.Remove(empleado);
        //    }
        //    return RedirectToAction(nameof(ListaEmpleado));
        //}
    }
}
