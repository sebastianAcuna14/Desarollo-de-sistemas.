using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;


namespace prototipo2.Controllers
{
    public class UsuarioController : Controller
    {
   
 

  
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
