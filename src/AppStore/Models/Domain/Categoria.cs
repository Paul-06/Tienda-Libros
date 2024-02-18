using System.ComponentModel.DataAnnotations;

namespace AppStore.Models.Domain;

public class Categoria
{
    [Key]
    [Required]
    public int Id { get; set; }
    public string? Nombre { get; set; }
    // Agregamos una colección de la clase relacionada (la relación aquí es de N:N)
    public virtual ICollection<Libro>? LibroRelationList { get; set; }
    // Agregamos una colección de la clase intermedia
    public virtual ICollection<LibroCategoria>? LibroCategoriaRelationList { get; set; }
    
}