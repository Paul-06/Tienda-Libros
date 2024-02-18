using AppStore.Models.Domain;
using AppStore.Models.DTO;
using AppStore.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;

namespace AppStore.Repositories.Implementation;

public class UserAuthenticationService : IUserAuthenticationService
{
    // Inyección de dependencias necesarias
    private readonly UserManager<ApplicationUser>? _userManager;
    private readonly SignInManager<ApplicationUser>? _signInManager;

    public UserAuthenticationService(UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Status> LoginAsync(LoginModel login)
    {
        var status = new Status();
        var user = await _userManager!.FindByNameAsync(login.Username!); // El método es asíncrono, así que usamos await

        // Si el usuario no existe
        if (user is null)
        {
            status.StatusCode = 0; // Significa false
            status.Message = "El username es inválido.";
            return status;
        }

        // Si la contraseña es incorrecta
        if (!await _userManager.CheckPasswordAsync(user, login.Password!))
        {
            status.StatusCode = 0;
            status.Message = "El password es inválido.";
            return status;
        }

        // Almacenamos el resultado del login
        var resultado = await _signInManager!.PasswordSignInAsync(user, login.Password!, true, false);

        // Si no fue exitoso
        if (!resultado.Succeeded)
        {
            status.StatusCode = 0;
            status.Message = "Las credenciales son incorrectas";
        }

        // Si llega aquí es porque fue exitoso
        status.StatusCode = 1; // Significa true
        status.Message = "Fue exitoso el login.";

        return status;
    }

    public async Task LogoutAsync()
    {
        await _signInManager!.SignOutAsync();
    }
}