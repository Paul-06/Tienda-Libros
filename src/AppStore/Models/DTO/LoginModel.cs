using System.ComponentModel.DataAnnotations;

namespace AppStore.Models.DTO;

// Modelo de user
public class LoginModel
{
    // Indicamos que ambos campos son requeridos
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
}