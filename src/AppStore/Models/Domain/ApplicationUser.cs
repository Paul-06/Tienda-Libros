using Microsoft.AspNetCore.Identity;

namespace AppStore.Models.Domain;

public class ApplicationUser : IdentityUser // Heredamos de la clase que se encuentra en el paquete Identity
{
    public string? Nombre { get; set; }
}