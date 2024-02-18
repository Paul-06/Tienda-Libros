using AppStore.Models.Domain;

namespace AppStore.Models.DTO;

public class LibroListVm // Esto es para paginar los resultados (libros)
{
    // IQueryable es el tipo por defecto que devuelve EF
    public IQueryable<Libro>? LibroList { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? Term { get; set; }
}
