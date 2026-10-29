using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Mensajes de enlace de datos en español
    var mensajes = options.ModelBindingMessageProvider;
    mensajes.SetValueMustNotBeNullAccessor(_ => "Este campo es obligatorio.");
    mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido.");
    mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
    mensajes.SetValueMustBeANumberAccessor(campo => $"El campo {campo} debe ser un número.");
    mensajes.SetMissingBindRequiredValueAccessor(campo => "Este campo es obligatorio.");
});

builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosDB")));

builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Datos iniciales: se cargan al iniciar la aplicación, después de aplicar las migraciones
// (dotnet ef database update), y solo si la tabla correspondiente está vacía.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();

    if (!await context.Categorias.AnyAsync())
    {
        context.Categorias.AddRange(
            new Categoria { Nombre = "Tecnología", Activo = true },
            new Categoria { Nombre = "Negocios", Activo = true },
            new Categoria { Nombre = "Arte y Cultura", Activo = true });
        await context.SaveChangesAsync();
    }

    if (!await context.Eventos.AnyAsync())
    {
        var categorias = await context.Categorias.AsNoTracking().ToDictionaryAsync(c => c.Nombre, c => c.Id);
        var hoy = DateTime.Today;

        context.Eventos.AddRange(
            new Evento
            {
                Titulo = "Conferencia de Inteligencia Artificial", Fecha = hoy.AddDays(20).AddHours(18),
                Lugar = "Auditorio Principal - Lima", Aforo = 200, Precio = 50.00m,
                CategoriaId = categorias["Tecnología"], Activo = true
            },
            new Evento
            {
                Titulo = "Taller práctico de ASP.NET Core", Fecha = hoy.AddDays(10).AddHours(9),
                Lugar = "Laboratorio 3 - Lima", Aforo = 3, Precio = 0.00m,
                CategoriaId = categorias["Tecnología"], Activo = true
            },
            new Evento
            {
                Titulo = "Seminario de Emprendimiento Digital", Fecha = hoy.AddDays(30).AddHours(16),
                Lugar = "Centro de Convenciones - San Isidro", Aforo = 150, Precio = 80.00m,
                CategoriaId = categorias["Negocios"], Activo = true
            },
            new Evento
            {
                Titulo = "Concierto de Música Andina", Fecha = hoy.AddDays(-5).AddHours(19).AddMinutes(30),
                Lugar = "Teatro Municipal - Lima", Aforo = 500, Precio = 35.00m,
                CategoriaId = categorias["Arte y Cultura"], Activo = true
            });
        await context.SaveChangesAsync();
    }

    if (!await context.Asistentes.AnyAsync())
    {
        context.Asistentes.AddRange(
            new Asistente { Nombres = "Ana Lucía Torres Ramos", Dni = "45781236", Correo = "ana.torres@correo.pe", Activo = true },
            new Asistente { Nombres = "Carlos Alberto Mendoza Ruiz", Dni = "70125489", Correo = "carlos.mendoza@correo.pe", Activo = true },
            new Asistente { Nombres = "María Fernanda Quispe Huamán", Dni = "48963257", Correo = "maria.quispe@correo.pe", Activo = true },
            new Asistente { Nombres = "Jorge Luis Paredes Salazar", Dni = "72458931", Correo = "jorge.paredes@correo.pe", Activo = true },
            new Asistente { Nombres = "Rosa Elena Vargas Chávez", Dni = "41236578", Correo = "rosa.vargas@correo.pe", Activo = true });
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
