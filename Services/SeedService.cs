using StoreOnline_Backend.Data;
using StoreOnline_Backend.DTOs;
using StoreOnline_Backend.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace StoreOnline_Backend.Services
{
    public class SeedService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SeedService> _logger;

        public SeedService(AppDbContext context, ILogger<SeedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, int Count, string Message)> SeedProductosAsync()
        {
            try
            {
                // Verificar si ya hay productos
                var existingCount = await _context.Productos.CountAsync();
                if (existingCount > 0)
                {
                    var msg = $"Ya existen {existingCount} productos en la base de datos. No se ejecutará el seed.";
                    _logger.LogInformation(msg);
                    return (false, existingCount, msg);
                }

                // Leer el archivo JSON
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "products.json");

                if (!File.Exists(jsonPath))
                {
                    var msg = $"❌ No se encontró el archivo JSON en: {jsonPath}";
                    _logger.LogError(msg);
                    return (false, 0, msg);
                }

                var jsonData = await File.ReadAllTextAsync(jsonPath);

                // Deserializar con opciones mejoradas
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };

                var productosResponse = JsonSerializer.Deserialize<ProductosResponse>(jsonData, options);

                if (productosResponse?.Products == null || !productosResponse.Products.Any())
                {
                    var msg = "⚠️ No se encontraron productos en el JSON";
                    _logger.LogWarning(msg);
                    return (false, 0, msg);
                }

                _logger.LogInformation($"📦 Se encontraron {productosResponse.Products.Count} productos en el JSON");

                // Mapear de JSON a tu modelo Producto
                var productos = productosResponse.Products.Select(p => new Producto
                {
                    ProductoId = p.Id,
                    Nombre = p.Title.Trim(),
                    Descripcion = p.Description.Trim(),
                    Categoria = p.Category.Trim(),
                    Marca = string.IsNullOrWhiteSpace(p.Brand) ? "Sin marca" : p.Brand.Trim(),
                    Precio = p.Price,
                    Calificacion = p.Rating,
                    Existencias = p.Stock,
                    ImagenUrl = p.Images?.FirstOrDefault() ?? p.Thumbnail,
                    MiniaturaUrl = p.Thumbnail,
                    EstadoDisponibilidad = p.AvailabilityStatus,
                    FechaRegistro = DateTime.UtcNow
                }).ToList();

                // Insertar en la base de datos
                _logger.LogInformation("💾 Insertando productos en la base de datos...");
                await _context.Productos.AddRangeAsync(productos);
                var registrosInsertados = await _context.SaveChangesAsync();

                var successMsg = $"✅ Se insertaron {registrosInsertados} productos exitosamente en PostgreSQL";
                _logger.LogInformation(successMsg);

                return (true, registrosInsertados, successMsg);
            }
            catch (JsonException jsonEx)
            {
                var msg = $"❌ Error al parsear JSON: {jsonEx.Message}";
                _logger.LogError(jsonEx, msg);
                return (false, 0, msg);
            }
            catch (DbUpdateException dbEx)
            {
                var msg = $"❌ Error al guardar en base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}";
                _logger.LogError(dbEx, msg);
                return (false, 0, msg);
            }
            catch (Exception ex)
            {
                var msg = $"❌ Error inesperado: {ex.Message}";
                _logger.LogError(ex, msg);
                return (false, 0, msg);
            }
        }

        // Método opcional para limpiar y re-seed
        public async Task<bool> ResetAndSeedAsync()
        {
            try
            {
                _logger.LogWarning("🗑️ Eliminando todos los productos existentes...");
                _context.Productos.RemoveRange(_context.Productos);
                await _context.SaveChangesAsync();

                var result = await SeedProductosAsync();
                return result.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear y hacer seed");
                return false;
            }
        }
    }
}