using Microsoft.AspNetCore.Mvc;
using prototipo2.Models; // Asegúrate de que el namespace del modelo sea correcto
using System.Linq;
namespace prototipo2.Controllers
{
    public class EmpleadoController : Controller
    {
        // Lista estática de empleados simulada
        private static List<Empleado> Empleados = new List<Empleado>
        {
            new() {
                Id = 1,
                NombreCompleto = "Carlos Pérez",
                Cedula = "101230456",
                Rol = "Gerente",
                Salario = 2500.00m,
                FechaContratacion = new DateTime(2020, 5, 10)
            },
            new() {
                Id = 2,
                NombreCompleto = "Laura Gómez",
                Cedula = "102349872",
                Rol = "Vendedor",
                Salario = 1200.00m,
                FechaContratacion = new DateTime(2022, 3, 15)
            }
        };

        // Acción Index que envía la lista a la vista


        public IActionResult CrearEmpleado()
        {
            return View();
        }
        public IActionResult ListaEmpleado()
        {
            return View(Empleados);
        }
        [HttpGet]
        public IActionResult EditarEmpleado(int id)
        {
            var empleado = Empleados.FirstOrDefault(e => e.Id == id);
            if (empleado == null) return NotFound();
            return View(empleado);


        }
        [HttpPost]
        public IActionResult CrearEmpleado(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                empleado.Id = Empleados.Any() ? Empleados.Max(e => e.Id) + 1 : 1;
                Empleados.Add(empleado);
                return RedirectToAction(nameof(ListaEmpleado));
            }
            return View(empleado);
        }


        public IActionResult EliminarEmpleado(int id)
        {
            var empleado = Empleados.FirstOrDefault(e => e.Id == id);
            if (empleado == null) return NotFound();
            return View(empleado);
        }





        [HttpPost]
        public IActionResult EditarEmpleado(int id, Empleado empleado)
        {
            if (id != empleado.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existente = Empleados.FirstOrDefault(e => e.Id == id);
                if (existente == null) return NotFound();

                existente.NombreCompleto = empleado.NombreCompleto;
                existente.Cedula = empleado.Cedula;
                existente.Rol = empleado.Rol;
                existente.Salario = empleado.Salario;
                existente.FechaContratacion = empleado.FechaContratacion;

                return RedirectToAction(nameof(ListaEmpleado));
            }
            return View(empleado);
        }
        

        public IActionResult Eliminar(int id)
        {
            var empleado = Empleados.FirstOrDefault(e => e.Id == id);
            if (empleado !=null)
            {
                Empleados.Remove(empleado);
            }
            return RedirectToAction(nameof(ListaEmpleado));
        }
    }
}
