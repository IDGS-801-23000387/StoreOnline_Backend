using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreOnline_Backend.Data;
using StoreOnline_Backend.Models;

namespace StoreOnline_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ /api/items?q=laptop
        [HttpGet("/api/items")]
        public async Task<IActionResult> BuscarProductos([FromQuery] string? q)
        {
            // Tomamos todos los productos
            var query = _context.Productos.AsQueryable();

            // Si hay texto de búsqueda, filtramos
            if (!string.IsNullOrEmpty(q))
            {
                var term = q.ToLower();
                query = query.Where(p =>
                    p.Nombre.ToLower().Contains(term) ||
                    p.Descripcion.ToLower().Contains(term) ||
                    p.Categoria.ToLower().Contains(term) ||
                    p.Marca.ToLower().Contains(term));
            }

            // Seleccionamos solo los datos que necesita la lista de resultados
            var resultados = await query
                .Select(p => new
                {
                    p.ProductoId,
                    p.Nombre,
                    p.Descripcion,
                    p.Categoria,
                    p.Marca,
                    p.Precio,
                    p.Calificacion,
                    p.Existencias,
                    p.ImagenUrl,
                    p.MiniaturaUrl
                })
                .ToListAsync();

            // Retornamos formato compatible con tu PWA
            return Ok(new
            {
                total = resultados.Count,
                items = resultados
            });
        }

        // ✅ /api/items/{id}
        [HttpGet("/api/items/{id}")]
        public async Task<IActionResult> ObtenerProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound(new { message = "Producto no encontrado" });

            return Ok(producto);
        }
    }
}
