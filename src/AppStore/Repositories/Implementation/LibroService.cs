using AppStore.Models.Domain;
using AppStore.Models.DTO;
using AppStore.Repositories.Abstract;

namespace AppStore.Repositories.Implementation;

public class LibroService : ILibroService
{
    // Aquí debe existir una instancia de EF Core
    // para tener acceso a las entidades y realizar
    // las operaciones CRUD y demás
    private readonly DatabaseContext _ctx; // La instancia de EF es nuestro proyecto es DbContext

    public LibroService(DatabaseContext context)
    {
        _ctx = context;
    }

    // Implementación de la interfaz
    public bool Add(Libro libro)
    {
        try
        {
            _ctx.Libros!.Add(libro);
            _ctx.SaveChanges();
            
            // Categorias a las que pertenece (esto es porque la relación es N:N)
            foreach (int categoriaId in libro.Categorias!)
            {
                var libroCategoria = new LibroCategoria
                {
                    LibroId = libro.Id,
                    CategoriaId = categoriaId
                };

                _ctx.LibroCategorias!.Add(libroCategoria);
            }

            _ctx.SaveChanges();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            // Buscamos el registro a eliminar (en este caso, un libro)
            var data = GetById(id);

            if (data is null)
            {
                return false;
            }

            // Eliminamos el registro de la tabla intermedia
            var libroCategorias = _ctx.LibroCategorias!.Where(a => a.LibroId == data.Id);
            _ctx.LibroCategorias!.RemoveRange(libroCategorias);

            // Eliminamos el registro de la tabla Libros
            _ctx.Libros!.Remove(data);

            // Guardamos los cambios en la bd
            _ctx.SaveChanges();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Libro GetById(int id)
    {
        return _ctx.Libros!.Find(id)!;
    }

    public List<int> GetCategoriaByLibroId(int libroId)
    {
        return _ctx.LibroCategorias!.Where(a => a.LibroId == libroId).Select(a => a.CategoriaId).ToList();
    }

    public LibroListVm List(string term = "", bool paging = false, int currentPage = 0)
    {
        // Este método hace una consulta de todos los registros de Libros y los irá paginando
        // (esto úsalo para menos de 10 000 registros)
        var data = new LibroListVm();
        var list = _ctx.Libros!.ToList(); // Lista de libros

        if (!string.IsNullOrEmpty(term)) // term es una varible que se usará para hacer búsquedas
        {
            term = term.ToLower(); // Ignoramos Mayús y Minús
            list = list.Where(a => a.Titulo!.ToLower().StartsWith(term)).ToList();
        }

        if (paging)
        {
            int pageSize = 5;
            int count = list.Count;
            int totalPages = (int) Math.Ceiling(count / (double) pageSize); // Ceiling devuelve el valor entero máximo de una división
            // Skip nos permite indicar desde que posición deseamos contar
            // Take nos permite establecer cuántos registros tomaremos para mostrar
            list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList(); // Recuerda que list es una colección de libros

            // Setteamos los atributos de data (LibroListVm)
            data.PageSize = pageSize;
            data.CurrentPage = currentPage;
            data.TotalPages = totalPages;
        }

        // Los libros tienen categorías en su interior
        foreach (var libro in list)
        {
            // Queremos que nos devuelva el nombre de las categorías
            var categorias = (
                from categoria in _ctx.Categorias
                join lc in _ctx.LibroCategorias!
                on categoria.Id equals lc.CategoriaId
                where lc.LibroId == libro.Id
                select categoria.Nombre
            ).ToList();

            // Almacenamos
            var categoriaNombres = string.Join(", ", categorias); // Drama, Horror, Accion...
            libro.CategoriasNames = categoriaNombres; // Esto se muestra en el index y el detalle del libro si no me equivoco
        }

        data.LibroList = list.AsQueryable();
        
        return data;
    }

    public bool Update(Libro libro)
    {
        try
        {
            // Removeremos las categorías que tenía el registro (libro)
            var categoriasParaEliminar = _ctx.LibroCategorias!.Where(a => a.LibroId == libro.Id);
            foreach (var categoria in categoriasParaEliminar)
            {
                _ctx.LibroCategorias!.Remove(categoria);
            }

            // Agregamos las nuevas categorías al registro, en sentido de actualización (libro)
            foreach (int categoriaId in libro.Categorias!)
            {
                var libroCategoria = new LibroCategoria { CategoriaId = categoriaId, LibroId = libro.Id };
                _ctx.LibroCategorias!.Add(libroCategoria);
            }

            // Hacemos la actualización de los otros datos del registro
            _ctx.Libros!.Update(libro);

            // Guardamos los cambios en la bd
            _ctx.SaveChanges();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}