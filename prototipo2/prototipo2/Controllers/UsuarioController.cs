using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
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
        public IActionResult Usuario()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registro(Login usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Id = Usuarios.Any() ? Usuarios.Max(e => e.Id) + 1 : 1;
                Usuarios.Add(usuario);
                return RedirectToAction(nameof(Login));
            }
            return View(usuario);
        }
        // ✅ Método corregido a GET para que funcione al hacer clic en el enlace
        [HttpGet]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            TempData["MensajeSesion"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Login", "Login");
        }
    }
}
