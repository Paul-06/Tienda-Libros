using AppStore.Models.Domain;
using AppStore.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppStore.Controllers;

[Authorize]
public class CategoriaController : Controller
{
    // Preparar inyección
    private readonly ICategoriaService? _categoriaService;

    // Constructor (se hace la inyección)
    public CategoriaController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    // Métodos API
    [HttpPost]
    public IActionResult Add(Categoria categoria)
    {
        if (!ModelState.IsValid)
        {
            return View(categoria);
        }

        var resultado = _categoriaService!.Add(categoria);

        if (resultado) {
            TempData["msg"] = "Se agregó la categoría exitosamente.";
            return RedirectToAction(nameof(Add)); // Redireccionar a la vista Add
        }

        TempData["msg"] = "Errores guardando la categoría.";
        return View(categoria);
    }

    [HttpPost]
    public IActionResult Edit(Categoria categoria)
    {
        if (!ModelState.IsValid) // Si hay argumentos vacíos
        {
            return View(categoria);
        }

        var resultado = _categoriaService!.Update(categoria);

        if (!resultado)
        {
            TempData["msg"] = "Errores al actualizar la categoría.";
            return View(categoria);
        }

        TempData["msg"] = "Se actualizó exitosamente la categoría.";
        
        return View(categoria);
    }

    // Métodos de navegación
    public IActionResult Add()
    {
        return View();
    }

    public IActionResult Edit(int id)
    {
        // Buscamos la categoría
        var categoria = _categoriaService!.GetById(id);

        // Lo mandamos hacia la vista
        return View(categoria);
    }

    public IActionResult CategoriaList()
    {
        // Almacenamos la lista de categorías
        var categorias = _categoriaService!.ListCategorias();

        return View(categorias);
    }

    public IActionResult Delete(int id)
    {
        _categoriaService!.Delete(id);

        return RedirectToAction(nameof(CategoriaList));
    }
}
