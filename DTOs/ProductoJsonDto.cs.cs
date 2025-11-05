// DTOs/ProductoJsonDto.cs
namespace StoreOnline_Backend.DTOs
{
    public class ProductosResponse
    {
        public List<ProductoJson> Products { get; set; } = new();
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }

    public class ProductoJson
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string AvailabilityStatus { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public string Thumbnail { get; set; } = string.Empty;
    }
}