using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppStore.Models.Domain;

public class Libro
{
    // Notaciones
    [Key]
    [Required]
    public int Id { get; set; }
    public string? Titulo { get; set; }
    public string? CreateDate { get; set; }
    public string? Imagen { get; set; }
    [Required]
    public string? Autor { get; set; }
    // Agregamos una colección de la clase relacionada (la relación aquí es de N:N)
    public virtual ICollection<Categoria>? CategoriaRelationList { get; set; }
    // Agregamos una colección de la clase intermedia
    public virtual ICollection<LibroCategoria>? LibroCategoriaRelationList { get; set; }

    // ID's de categorías
    // Evitaremos que esto se tome en cuenta para el mapeo de la BD
    [NotMapped]
    public List<int>? Categorias { get; set; }

    // Nombres de las categorías
    [NotMapped]
    public string? CategoriasNames { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; } // Para poder manejar un archivo de imagen (esto porque estará asociado al libro)

    // Cargar los datos de las categorías
    [NotMapped]
    public IEnumerable<SelectListItem>? CategoriasList { get; set; }

    // Cargar una lista de categorías que pertenezca a "x" libro
    [NotMapped]
    public MultiSelectList? MultiCategoriasList { get; set; }

}