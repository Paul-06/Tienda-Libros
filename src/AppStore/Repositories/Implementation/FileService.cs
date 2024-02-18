using AppStore.Repositories.Abstract;

namespace AppStore.Repositories.Implementation;

public class FileService : IFileService
{
    /*
    Para acceder a la carpeta wwwroot/Uploads, necesitamos
    una instancia de WebHostEnvironment
    */
    private readonly IWebHostEnvironment? _environment;

    // Inyectamos
    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Tuple<int, string> SaveImage(IFormFile imageFile)
    {
        try
        {
            // Capturar la ubicación del directorio wwwroot
            var wwwPath = _environment!.WebRootPath;

            // Ingresar al path de Uploads
            var path = Path.Combine(wwwPath, "Uploads");

            if (!Directory.Exists(path)) // Por si el directorio no existe
            {
                Directory.CreateDirectory(path);
            }

            // Sólo queremos archivos de imágenes (png, jpg, jpeg)
            var ext = Path.GetExtension(imageFile.FileName);

            // Extensiones permitidas
            var allowedExtensions = new string[] {".jpg", ".png", ".jpeg"};

            // Si no es de la extensión deseada
            if (!allowedExtensions.Contains(ext))
            {
                var message = $"Sólo están permitidas los archivos con extensiones {allowedExtensions}";
                return new Tuple<int, string>(0, message);
            }

            // Por si el nombre del archivo ya existe en la bd (generamos un nuevo nombre por así decirlo)
            var uniqueString = Guid.NewGuid().ToString();
            var newFileName = uniqueString + ext; // Concatenamos la extensión

            // Agregar la ubicación donde estará la imagen
            var fileWithPath = Path.Combine(path, newFileName);

            // Guardar la imagen (copiar la imagen al directorio)
            var stream = new FileStream(fileWithPath, FileMode.Create);

            // Para que se ejecute
            imageFile.CopyTo(stream);
            stream.Close();

            return new Tuple<int, string>(1, newFileName); // Pasamos el nombre del archivo para vincularlo con el registro del libro

        }
        catch (Exception)
        {
            return new Tuple<int, string>(0, "Errores al guardar la imagen.");
        }
    }

    public bool DeleteImage(string imageFileName)
    {
        try
        {
            var wwwPath = _environment!.WebRootPath;
            var path = Path.Combine(wwwPath, "Uploads\\", imageFileName); // Ruta de la imagen a eliminar

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }

            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}