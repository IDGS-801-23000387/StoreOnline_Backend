using StoreOnline_Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreOnline_Backend.Models
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // usamos el ID del JSON
        public int ProductoId { get; set; }

        [Required, MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Categoria { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Marca { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public double Calificacion { get; set; }

        public int Existencias { get; set; }

        [MaxLength(500)]
        public string ImagenUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string MiniaturaUrl { get; set; } = string.Empty;

        [MaxLength(50)]
        public string EstadoDisponibilidad { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public ICollection<Venta>? Ventas { get; set; }
    }
}
