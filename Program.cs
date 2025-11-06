using Microsoft.EntityFrameworkCore;
using StoreOnline_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine(" No se encontró la cadena de conexión a la base de datos.");
}
else
{
    Console.WriteLine($" Conectando a la base de datos: {connectionString}");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

 
builder.Services.AddCors(options =>
{
    options.AddPolicy("netlify", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",                  
            "https://storeonlines.netlify.app"        
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreOnline API v1");
        c.RoutePrefix = string.Empty;  
    });
}

app.UseHttpsRedirection();
app.UseCors("netlify");
app.UseAuthorization();
app.MapControllers();

app.Run();
