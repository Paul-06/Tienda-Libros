using AppStore.Models.Domain;

namespace AppStore.Models.DTO;

public class CategoriaListVm // Esto es para paginar los resultados (libros)
{
    // IQueryable es el tipo por defecto que devuelve EF
    public IQueryable<Categoria>? CategoriaList { get; set; }
}
