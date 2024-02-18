using Microsoft.EntityFrameworkCore;
using AppStore.Models.Domain;
using Microsoft.AspNetCore.Identity;
using AppStore.Repositories.Abstract;
using AppStore.Repositories.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Inyección de dependencias
/*
AddScoped creará una instancia de LibroService (y demás)
con la interfaz ILibroService (y demás).
*/ 
builder.Services.AddScoped<ILibroService, LibroService>();
// También agregaremos el servicio para la autenticación (inyección)
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
// Vamos a agregar nuestros nuevos servicios (FileService y CategoriaService)
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();


builder.Services.AddDbContext<DatabaseContext> (opt => { // Por defecto, se crea una instancia de DbContext
    opt.LogTo(Console.WriteLine, new [] {
        DbLoggerCategory.Database.Command.Name},
        LogLevel.Information).EnableSensitiveDataLogging();

        opt.UseSqlite(builder.Configuration.GetConnectionString("SqliteDatabase")); // Revisar appsettings.json
});

// Inyección necesaria (Ver LoadDatabase)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<DatabaseContext>()
.AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Agregamos esto para el login

app.UseAuthorization();

app.MapControllerRoute( // Enrutamiento del controlador
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}" // El URL que se visualiza
);

// Lógica para llamar a LoadDatabase
using (var ambiente = app.Services.CreateScope())
{
    var services = ambiente.ServiceProvider;

    try
    {
        // Instaciamos las clases para los parámetros de de LoadDatabase
        var context = services.GetRequiredService<DatabaseContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Creamos la BD
        await context.Database.MigrateAsync(); // También se puede usar un comando dotnet

        // Hacemos la inserción
        await LoadDatabase.InsertarData(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logging = services.GetRequiredService<ILogger<Program>>();
        logging.LogError(ex, "Ocurrió un error en la inserción de datos.");
    }
}


app.Run();
