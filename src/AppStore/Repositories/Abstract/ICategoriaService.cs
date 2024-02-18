using AppStore.Models.Domain;
using AppStore.Models.DTO;

namespace AppStore.Repositories.Abstract;

public interface ICategoriaService
{
    // Retornar una lista envuelta en un IQueryable
    IQueryable<Categoria> List();
    CategoriaListVm ListCategorias();
    bool Add(Categoria categoria);
    bool Update(Categoria categoria);
    Categoria GetById(int id);
    bool Delete(int id);
}
