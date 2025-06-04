using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;

namespace prototipo2.Controllers
{
    public class LoginController : Controller
    {
        private const string StaticUsername = "admin";
        private const string StaticPassword = "1234";

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
                if (model.Username == StaticUsername
                    && model.Password == StaticPassword)
                {
                    TempData["UsuarioAutenticado"] = model.Username;
                    return RedirectToAction("Home", "Index"); // Cambia si tienes otra vista
                }

                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            }

            return View(model);
        }
    }
}
