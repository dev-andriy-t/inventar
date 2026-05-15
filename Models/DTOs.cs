using System.Collections.Generic;

namespace RetailingApp.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int? ManufacturerId { get; set; }
        public int? LocationId { get; set; }
        public List<int> CategoryIds { get; set; } = new();
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class ManufacturerDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class LocationDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
