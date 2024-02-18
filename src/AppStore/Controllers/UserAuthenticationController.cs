using AppStore.Models.DTO;
using AppStore.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc; // De aquí heredamos Controller

namespace AppStore.Controllers;

public class UserAuthenticationController : Controller
{
    // Inyección de Repository
    private readonly IUserAuthenticationService? _authService;

    // Aquí se hace la inyección
    public UserAuthenticationController(IUserAuthenticationService authService) // Solicitamos un objeto de tipo IUserAuth...
    {
        _authService = authService;
    }

    // Pista existente
    public IActionResult Login()
    {
        return View();
    }

    // Método de tipo API
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel login)
    {
        // Manejar el error en caso de argumentos vacíos
        if (!ModelState.IsValid)
        {
            return View(login);
        }

        var resultado = await _authService!.LoginAsync(login); // Devuelve un objeto de tipo Status

        if (resultado.StatusCode == 1) // Si es verdadero
        {
            return RedirectToAction("Index", "Home"); // view, controller => URL (redireccionar a Index)
        }
        else 
        {
            TempData["msg"] = resultado.Message; 
            return RedirectToAction(nameof(Login)); // Redireccionar al login (llama a una pista existente)
        }
    }

    // Cerrar sesión
    public async Task<IActionResult> Logout()
    {
        await _authService!.LogoutAsync();
        return RedirectToAction(nameof(Login)); // Redireccionar al login
    }

}