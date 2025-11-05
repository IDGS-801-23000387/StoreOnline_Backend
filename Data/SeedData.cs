using System.Text.Json;
using StoreOnline_Backend.Models;

namespace StoreOnline_Backend.Data
{
    public static class SeedData
    {
        public static void CargarProductos(AppDbContext context)
        {
            // Evitar duplicados si ya hay productos
            if (context.Productos.Any())
            {
                Console.WriteLine("⚠️ Ya existen productos en la base de datos. Sembrado omitido.");
                return;
            }

            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "products.json");
                Console.WriteLine($"📁 Buscando archivo JSON en: {filePath}");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"❌ No se encontró el archivo {filePath}");
                    return;
                }

                var jsonData = File.ReadAllText(filePath);
                var jsonDoc = JsonDocument.Parse(jsonData);
                var productosJson = jsonDoc.RootElement.GetProperty("products");

                var productos = new List<Producto>();

                foreach (var item in productosJson.EnumerateArray())
                {
                    var producto = new Producto
                    {
                        ProductoId = item.GetProperty("id").GetInt32(),
                        Nombre = item.GetProperty("title").GetString() ?? string.Empty,
                        Descripcion = item.GetProperty("description").GetString() ?? string.Empty,
                        Categoria = item.GetProperty("category").GetString() ?? string.Empty,
                        Marca = item.TryGetProperty("brand", out var brandProp) ? brandProp.GetString() ?? string.Empty : string.Empty,
                        Precio = item.GetProperty("price").GetDecimal(),
                        Calificacion = item.GetProperty("rating").GetDouble(),
                        Existencias = item.GetProperty("stock").GetInt32(),
                        ImagenUrl = item.GetProperty("images")[0].GetString() ?? string.Empty,
                        MiniaturaUrl = item.GetProperty("thumbnail").GetString() ?? string.Empty,
                        EstadoDisponibilidad = item.GetProperty("availabilityStatus").GetString() ?? "Disponible",
                        FechaRegistro = DateTime.UtcNow
                    };
                    productos.Add(producto);
                }

                Console.WriteLine($"📦 Se encontraron {productos.Count} productos en el archivo JSON");

                context.Productos.AddRange(productos);
                context.SaveChanges();

                Console.WriteLine($"✅ Se insertaron {productos.Count} productos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cargar productos: {ex}");
            }
        }
    }
}
