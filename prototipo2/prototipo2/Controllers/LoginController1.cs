using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class LoginController : Controller
    {

        private static List<Login> Usuario = new List<Login>
        {
            new() {
                Id = 1,
                Username = "admin@gmail.com",
                Password = "1234",
              },

            new() {
                Id = 2,
                Username = "vendedor@gmail.com",
                Password = "5678",
              }
        };



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var usuario = Usuario.FirstOrDefault(u => u.Username == model.Username &&
                u.Password == model.Password);
                if (usuario != null)
                {
                    TempData["UsuarioAutenticado"] = model.Username;
                    TempData["UsuarioId"] = usuario.Id;
                    return RedirectToAction("Index", "Home");
                }
               
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Recuperar(string email)
        {
            var usuario = Usuario.FirstOrDefault(u => u.Username == email);

            if (usuario != null)
            {
                ViewBag.UsuarioEncontrado = true;
                ViewBag.Email = email;
            }
            else
            {
                ViewBag.Mensaje = "Correo no registrado.";
            }

            return View();
        }

        [HttpPost]
        public IActionResult CambiarContrasena(string email, string nuevaContrasena)
        {
            var usuario = Usuario.FirstOrDefault(u => u.Username == email);

            if (usuario != null)
            {
                usuario.Password = nuevaContrasena;
                TempData["MensajeExito"] = "Contraseña actualizada correctamente. Inicia sesión con tu nueva contraseña.";
                return RedirectToAction("Login");
            }

            ViewBag.Mensaje = "Error: Usuario no encontrado.";
            return View("Recuperar");
        }


    }
}
