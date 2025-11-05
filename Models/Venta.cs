using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreOnline_Backend.Models
{
    [Table("Ventas")]
    public class Venta
    {
        [Key]
        public int VentaId { get; set; }

        [ForeignKey("Producto")]
        public int ProductoId { get; set; }

        [Required, MaxLength(200)]
        public string NombreProducto { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; } = 1;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total => PrecioUnitario * Cantidad;

        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        public Producto? Producto { get; set; }
    }
}
