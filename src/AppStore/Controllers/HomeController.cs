using AppStore.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc; // De aquí heredamos Controller

namespace AppStore.Controllers;

public class HomeController : Controller // Heredamos de Controller
{
    // Inyectamos IService
    private readonly ILibroService? _libroService;

    // Constructor (aquí solicitamos un objeto de tipo ILibroService)
    public HomeController(ILibroService libroService)
    {
        _libroService = libroService;
    }

    // Usualmente, los metodos devuelven una página
    // (página cuyo nombre es igual al del método)
    public IActionResult Index(string term = "", int currentPage = 1)
    {
        var libros = _libroService!.List(term, true, currentPage); // Devuelve un tipo LibroListVm
        return View(libros);
    }

    // Devolverá una página para los detalles de un libro
    public IActionResult LibroDetail(int libroId)
    {
        var libro = _libroService!.GetById(libroId);
        return View(libro);
    }

    // Redireccionamiento a about
    public IActionResult About()
    {
        return View();
    }
}