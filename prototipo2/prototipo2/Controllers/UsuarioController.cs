using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;
using System.Linq;

namespace prototipo2.Controllers
{
    public class UsuarioController : Controller
    {
        private static List<Usuario> Usuarios = new List<Usuario>();
        private static int nextId = 1;

        [HttpGet]
        public IActionResult Registro()
        {
            return View(new Usuario());
        }
        public IActionResult Usuario()
        {
            return View(Usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (Usuarios.Any(u => u.Email == usuario.Email))
                {
                    ModelState.AddModelError("Email", "Este correo electrónico ya está registrado");
                    return View(usuario);
                }

                usuario.Id = nextId++;
                Usuarios.Add(usuario);

                TempData["MensajeExito"] = "¡Registro exitoso! Ahora puedes iniciar sesión.";
                return RedirectToAction("Login", "Login");
            }

            return View(usuario);
        }
    }
}
