using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppStore.Models.Domain;

public class DatabaseContext : IdentityDbContext<ApplicationUser> // Incluir el modelo de seguridad que viene en Identity (indicamos la clase que representará a los usuarios)
{
    // Solicitará la cadena de conexión
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Definir la relación N:N
        builder.Entity<Libro>()
        .HasMany(x => x.CategoriaRelationList)
        .WithMany(y => y.LibroRelationList)
        .UsingEntity<LibroCategoria>(
            j => j
            .HasOne(p => p.Categoria)
            .WithMany(p => p.LibroCategoriaRelationList)
            .HasForeignKey(p => p.CategoriaId), // Categoria
            j => j
            .HasOne(p => p.Libro)
            .WithMany(p => p.LibroCategoriaRelationList)
            .HasForeignKey(p => p.LibroId), // Libro
            j =>
            {
                j.HasKey(t => new { t.LibroId, t.CategoriaId }); // Establecer que ambos campos son PK
            }
        );
    }

    // Entidades (que a futuro serán tablas)
    public DbSet<Categoria>? Categorias { get; set; } // Definimos el nombre en plura porque ese nombre tendrá en la base de datos
    public DbSet<Libro>? Libros { get; set; }
    public DbSet<LibroCategoria>? LibroCategorias { get; set; }
}