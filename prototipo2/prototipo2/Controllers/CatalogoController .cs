using Microsoft.AspNetCore.Mvc;

public class CatalogoController : Controller
{
    public IActionResult Index()
    {
        return View(); // Esto mostrará Views/Catalogo/Index.cshtml
    }
}