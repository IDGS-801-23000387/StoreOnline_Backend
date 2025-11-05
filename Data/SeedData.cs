using System.Text.Json;
using StoreOnline_Backend.Models;

namespace StoreOnline_Backend.Data
{
    public class SeedData
    {
        public static void CargarProductos(AppDbContext context)
        {
            try
            {
                if (context.Productos.Any())
                {
                    Console.WriteLine("✅ La tabla Productos ya contiene datos. No se insertaron nuevos registros.");
                    return;
                }

                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "Data", "products.json");
                if (!File.Exists(ruta))
                {
                    Console.WriteLine("⚠️ No se encontró el archivo products.json en la carpeta Data.");
                    return;
                }

                Console.WriteLine($"📂 Leyendo archivo JSON: {ruta}");

                // Leer y limpiar posibles caracteres invisibles
                var json = File.ReadAllText(ruta).Trim('\uFEFF', '\u200B', ' ', '\t', '\r', '\n');

                var opciones = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };

                // Intentar deserializar directamente como objeto con "products"
                using var doc = JsonDocument.Parse(json, new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                });

                if (!doc.RootElement.TryGetProperty("products", out var productosJson))
                {
                    Console.WriteLine("⚠️ No se encontró la propiedad 'products' en el JSON.");
                    return;
                }

                var lista = JsonSerializer.Deserialize<List<ProductTemp>>(productosJson.GetRawText(), opciones);

                if (lista == null || lista.Count == 0)
                {
                    Console.WriteLine("⚠️ No se encontraron productos válidos en el archivo JSON.");
                    return;
                }

                foreach (var p in lista)
                {
                    var producto = new Producto
                    {
                        ProductoId = p.id,
                        Nombre = p.title ?? "Sin nombre",
                        Descripcion = p.description ?? "",
                        Categoria = p.category ?? "",
                        Marca = p.brand ?? "",
                        Precio = Convert.ToDecimal(p.price),
                        Calificacion = p.rating,
                        Existencias = p.stock,
                        ImagenUrl = p.thumbnail ?? p.images?.FirstOrDefault() ?? "",
                        MiniaturaUrl = p.images?.FirstOrDefault() ?? "",
                        EstadoDisponibilidad = p.stock > 0 ? "Disponible" : "Agotado",
                        FechaRegistro = DateTime.UtcNow
                    };

                    context.Productos.Add(producto);
                }

                context.SaveChanges();
                Console.WriteLine($"✅ Se insertaron {lista.Count} productos correctamente en la base de datos.");
            }
            catch (JsonException ex)
            {
                Console.WriteLine("❌ Error al deserializar el JSON:");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error general al cargar los productos:");
                Console.WriteLine(ex.Message);
            }
        }

        // Clase auxiliar para mapear datos del JSON
        private class ProductTemp
        {
            public int id { get; set; }
            public string? title { get; set; }
            public string? description { get; set; }
            public string? category { get; set; }
            public string? brand { get; set; }
            public double price { get; set; }
            public double rating { get; set; }
            public int stock { get; set; }
            public string? thumbnail { get; set; }
            public List<string>? images { get; set; }
        }
    }
}
