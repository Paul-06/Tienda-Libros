using Microsoft.AspNetCore.Identity;

namespace AppStore.Models.Domain;

public class LoadDatabase
{
    public static async Task InsertarData(
    DatabaseContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
    {
        // Agregar roles
        if (!roleManager.Roles.Any()) // Si no tiene data (se está haciendo una negación)
        {
            await roleManager.CreateAsync(new IdentityRole("ADMIN"));
        }

        // Agregar usuarios
        if (!userManager.Users.Any())
        {
            var usuario = new ApplicationUser {
                Nombre = "Jean Vasquez",
                Email = "jean.vasquez.dev@gmail.com",
                UserName = "jean.vasquez"
            };

            await userManager.CreateAsync(usuario, "PasswordJeanVasquez123$");

            // Asignar rol
            await userManager.AddToRoleAsync(usuario, "ADMIN");
        }

        // Agregar categorias
        if (!context.Categorias!.Any())
        {
            await context.Categorias!.AddRangeAsync(
                new Categoria { Nombre = "Drama" },
                new Categoria { Nombre = "Comedia" },
                new Categoria { Nombre = "Accion" },
                new Categoria { Nombre = "Terror" },
                new Categoria { Nombre = "Aventura" }
            );

            // Disparamos los cambios en la bd
            await context.SaveChangesAsync();
        }

        // Agregar libros
        if (!context.Libros!.Any())
        {
            await context.Libros!.AddRangeAsync(
                new Libro {
                    Titulo = "El Quijote De La Mancha",
                    CreateDate = "06/06/2020",
                    Imagen = "quijote.jpg",
                    Autor = "Miguel De Cervantes"
                },
                new Libro {
                    Titulo = "Harry Potter",
                    CreateDate = "06/01/2021",
                    Imagen = "harry.jpg",
                    Autor = "J. K. Rowling"
                }
            );

            await context.SaveChangesAsync();
        }

        // Agregar LibroCategorias
        if (!context.LibroCategorias!.Any())
        {
            await context.LibroCategorias!.AddRangeAsync(
                new LibroCategoria { CategoriaId = 1, LibroId = 1 },
                new LibroCategoria { CategoriaId = 2, LibroId = 2 }
            );

            await context.SaveChangesAsync();
        }
    }
}