using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RetailingApp.Entities;

namespace RetailingApp.Entities
{
    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }
        public string? Name {get; set;}

        [JsonIgnore] 
        public List<Product> Products { get; set; } = new();
        public Manufacturer() {}

        public Manufacturer(string name) {
            Name = name;
        }
    }
}