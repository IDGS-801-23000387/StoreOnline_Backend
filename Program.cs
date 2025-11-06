using Microsoft.EntityFrameworkCore;
using StoreOnline_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreOnline API v1");
    c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();
app.UseCors("netlify");
app.UseAuthorization();
app.MapControllers();
app.Run();
