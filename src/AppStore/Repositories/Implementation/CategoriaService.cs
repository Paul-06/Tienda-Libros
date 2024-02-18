using AppStore.Models.Domain;
using AppStore.Models.DTO;
using AppStore.Repositories.Abstract;

namespace AppStore.Repositories.Implementation;

public class CategoriaService : ICategoriaService
{
    // Si queremos devolver una lista de valores de la BD, debemos usar nuestro DbContext
    private readonly DatabaseContext _ctx;

    // Inyectando
    public CategoriaService(DatabaseContext context)
    {
        _ctx = context;
    }

    public bool Add(Categoria categoria)
    {
        _ctx.Categorias!.Add(categoria); // _ctx -> DbContext || Categorias -> Entidad
        _ctx.SaveChanges(); // Guardamos los cambios

        return true;
    }

    public bool Delete(int id)
    {
        try
        {
            // Buscamos la categoría a eliminar
            var data = GetById(id);

            // Si data es nula
            if (data is null)
            {
                return false;
            }

            // Actualizar los registro de libro quitándoles las categorías que serán eliminadas
            // Por revisar

            // Eliminamos el registro o los registro correspondientes ligados a la categoría
            // en la tabla intermedia
            var libroCategorias = _ctx.LibroCategorias!.Where(a => a.CategoriaId == data.Id); // Devuelve un IQueryable<LibroCategoria>
            _ctx.LibroCategorias!.RemoveRange(libroCategorias);

            // Eliminamos la categoría
            _ctx.Categorias!.Remove(data);

            // Guardamos los cambios
            _ctx.SaveChanges();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Categoria GetById(int id)
    {
        return _ctx.Categorias!.Find(id)!;
    }

    public IQueryable<Categoria> List()
    {
        return _ctx.Categorias!.AsQueryable();
    }

    public CategoriaListVm ListCategorias()
    {
        var categoriaListVm = new CategoriaListVm {
            CategoriaList = List()
        };

        return categoriaListVm;
    }

    public bool Update(Categoria categoria)
    {
        try
        {
            // Actualizar el registro
            _ctx.Categorias!.Update(categoria);

            // Guardamos los cambios
            _ctx.SaveChanges();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
