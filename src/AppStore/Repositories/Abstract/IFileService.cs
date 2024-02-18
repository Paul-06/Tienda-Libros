namespace AppStore.Repositories.Abstract;

public interface IFileService
{
    // Almacenar una imagen
    public Tuple<int, string> SaveImage(IFormFile imageFile);

    // Eliminar una imagen
    public bool DeleteImage(string imageFileName);
}