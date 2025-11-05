using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreOnline_Backend.Data;
using StoreOnline_Backend.Models;

namespace StoreOnline_Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ POST /api/addSale
        [HttpPost("addSale")]
        public async Task<IActionResult> RegistrarVenta([FromBody] Venta venta)
        {
            var producto = await _context.Productos.FindAsync(venta.ProductoId);
            if (producto == null)
                return NotFound(new { message = "Producto no encontrado" });

            // Validar stock
            if (producto.Existencias < venta.Cantidad)
                return BadRequest(new { message = "No hay existencias suficientes" });

            // Registrar venta
            venta.NombreProducto = producto.Nombre;
            venta.PrecioUnitario = producto.Precio;
            venta.FechaVenta = DateTime.UtcNow;

            _context.Ventas.Add(venta);

            // Actualizar stock
            producto.Existencias -= venta.Cantidad;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Venta registrada correctamente" });
        }

        // ✅ GET /api/sales
        [HttpGet("sales")]
        public async Task<IActionResult> ObtenerVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Producto)
                .OrderByDescending(v => v.FechaVenta)
                .Select(v => new
                {
                    v.VentaId,
                    v.ProductoId,
                    v.NombreProducto,
                    v.PrecioUnitario,
                    v.Cantidad,
                    v.Total,
                    v.FechaVenta
                })
                .ToListAsync();

            return Ok(ventas);
        }
    }
}
