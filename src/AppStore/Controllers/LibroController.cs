using AppStore.Models.Domain;
using AppStore.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppStore.Controllers;

// Estos métodos deben llevarse a cabo siempre y cuando
// el usuario este logeado (autenticado)
[Authorize]
public class LibroController : Controller
{
    // Inyectar servicios
    private readonly ILibroService? _libroService;
    private readonly IFileService? _fileService;
    private readonly ICategoriaService? _categoriaService;

    // Constructor (aquí se hace la inyección, ya que se reciben los objetos de los servicios (instancias))
    public LibroController(ILibroService libroService, IFileService fileService, ICategoriaService categoriaService)
    {
        _libroService = libroService;
        _fileService = fileService;
        _categoriaService = categoriaService;
    }

    // Métodos API
    [HttpPost]
    public IActionResult Add(Libro libro)
    {
        // Lista de categorías totales (todas las categorías)
        libro.CategoriasList = _categoriaService!.List()
        .Select(a => new SelectListItem {Text = a.Nombre, Value = a.Id.ToString()});

        if (!ModelState.IsValid) // Si los argumentos (campos) son vacíos
        {
            return View(libro);
        }

        if (libro.ImageFile != null)
        {
            var resultado = _fileService!.SaveImage(libro.ImageFile);

            if (resultado.Item1 == 0) // Si es falso (item1 es el primer valor de la tupla))
            {
                TempData["msg"] = "La imagen no pudo guardarse exitosamente.";
                return View(libro);
            }

            var imagenName = resultado.Item2; // El método SaveImage() devuelve un tupla con un número (0 - 1) y el nombre del archivo (ex. fileName.png)
            libro.Imagen = imagenName;
        }

        var resultadoLibro = _libroService!.Add(libro);

        if (resultadoLibro)
        {
            TempData["msg"] = "Se agregó el libro exitosamente.";
            return RedirectToAction(nameof(Add)); // Redireccionar a la vista Add
        }

        // Si hay errores al guardar el libro
        TempData["msg"] = "Errores guardando el libro.";
        return View(libro);
    }

    [HttpPost]
    public IActionResult Edit(Libro libro)
    {
        // Definir las categorías seleccionadas del libro
        var categoriasDeLibro = _libroService!.GetCategoriaByLibroId(libro.Id); // IDs de categorías
        // MultiCategoriaList
        var multiSelectListCategorias = new MultiSelectList(_categoriaService!.List(), "Id", "Nombre", categoriasDeLibro);
        libro.MultiCategoriasList = multiSelectListCategorias; // Setteamos la propiedad

        // Validar
        if (!ModelState.IsValid) // Si tiene argumentos vacíos
        {
            return View(libro);
        }

        // En caso sea correcto (continua el flujo)
        if (libro.ImageFile != null)
        {
            var fileResultado = _fileService!.SaveImage(libro.ImageFile);

            // Si la inserción de la nueva imagen es erróneo
            if (fileResultado.Item1 == 0)
            {
                TempData["msg"] = "La imagen no fue guardada.";
                return View(libro);
            }

            // Si se guarda exitosamente la imagen
            var imagenName = fileResultado.Item2; // Recordar que el nombre de la imagen se guarda en el item2 de la tupla
            libro.Imagen = imagenName;
        }

        var resultadoLibro = _libroService.Update(libro); // Devuelve true o false

        if (!resultadoLibro)
        {
            TempData["msg"] = "Errores al actualizar el libro.";
            return View(libro);
        }

        TempData["msg"] = "Se actualizó exitosamente el libro.";
        
        return View(libro);
    }


    // Métodos de navegación (los que normalmente devuelven una página)
    public IActionResult Add()
    {
        var libro = new Libro();
        libro.CategoriasList = _categoriaService!.List()
        .Select(a => new SelectListItem {Text = a.Nombre, Value = a.Id.ToString()});


        return View(libro); // Para enviar la lista de categorías en el view
    }

    public IActionResult Edit(int id)
    {
        // Buscar el libro en la bd
        var libro = _libroService!.GetById(id);
        // Categorías asignadas en el libro a buscar (editar)
        var categoriasDeLibro = _libroService.GetCategoriaByLibroId(id); // IDs de categorías

        // Crea una nueva lista de selección múltiple con las categorías obtenidas del servicio.
        // El "Id" de cada categoría se usa como valor y el "Nombre" como texto mostrado.
        // Las categorías inicialmente seleccionadas son las que están en 'categoriasDeLibro'.
        var multiSelectListCategorias = new MultiSelectList(_categoriaService!.List(), "Id", "Nombre", categoriasDeLibro);

        // Setteamos el valor
        libro.MultiCategoriasList = multiSelectListCategorias;

        return View(libro);
    }

    public IActionResult LibroList()
    {
        var libros = _libroService!.List(); // Retornar la lista de libros desde la bd
        return View(libros);
    }

    public IActionResult Delete(int id)
    {
        _libroService!.Delete(id);
        return RedirectToAction(nameof(LibroList)); // No devolverá una página, sino un redireccionamiento a la lista de libros
    }

    // Queda en mis manos realizar el mantenimiento de las categorías (lo borro al terminar esto)
}
