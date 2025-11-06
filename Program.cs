using Microsoft.EntityFrameworkCore;
using StoreOnline_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================
// CONFIGURAR CONEXIÓN A BD (PostgreSQL - Railway)
// ============================

// Obtiene la cadena de conexión del appsettings.json o de las variables de entorno
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("⚠️ No se encontró la cadena de conexión a la base de datos.");
}
else
{
    Console.WriteLine($"🔌 Conectando a la base de datos: {connectionString}");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// ============================
// AGREGAR SERVICIOS DEL API
// ============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================
// CONFIGURAR CORS (permite acceso desde tu PWA en Netlify)
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("netlify", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",                 // 👈 permite tu React local
            "https://storeonlines.netlify.app"       // 👈 tu dominio en producción (sin / al final)
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


var app = builder.Build();

// ============================
// APLICAR MIGRACIONES Y SEMBRAR DATOS DESDE JSON
// ============================
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Console.WriteLine("🏗️ Aplicando migraciones pendientes...");
        db.Database.Migrate(); // Crea tablas si no existen

        Console.WriteLine("🌱 Cargando productos desde JSON...");
        SeedData.CargarProductos(db); // Inserta los productos del archivo JSON
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error al aplicar migraciones o cargar datos:");
        Console.WriteLine(ex.ToString()); // imprime detalles completos del error
    }
}

// ============================
// CONFIGURAR EL PIPELINE
// ============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreOnline API v1");
        c.RoutePrefix = string.Empty; // Muestra Swagger directamente en la raíz del dominio
    });
}

app.UseHttpsRedirection();
app.UseCors("netlify");
app.UseAuthorization();
app.MapControllers();

app.Run();
