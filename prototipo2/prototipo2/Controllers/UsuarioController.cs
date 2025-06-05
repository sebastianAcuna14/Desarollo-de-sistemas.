using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class UsuarioController : Controller
    {
        private static List<Login> Usuarios = new List<Login>();



        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Registro(Login model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Usuarios.Any() ? Usuarios.Max(e => e.Id) + 1 : 1;
                Usuarios.Add(model);

                HttpContext.Session.SetString("Usuario", model.Username ?? "");
                HttpContext.Session.SetInt32("UsuarioId", model.Id);

                return RedirectToAction("Usuario");
            }

            return View(model);

        }

        public IActionResult Usuario()
        {
            var usuario = HttpContext.Session.GetString("Usuario");

            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Usuario = usuario;
            return View();
        }


        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login", "Login");
        }


    }
}
