using Microsoft.AspNetCore.Mvc;
using prototipoTienda.Models; // Asegúrate de que el namespace del modelo sea correcto
using System.Collections.Generic;

namespace prototipoTienda.Controllers
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
        
        public IActionResult Index()
        {
            return View(Empleados);
        }
    }
}
