using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetailingApp.Entities;

namespace RetailingApp.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Name {get; set;}
        public double Price {get; set;} = 0;
        public int Quantity {get; set;} = 0;

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public List<Category> Categories { get; set; } = new();
        public Manufacturer? Manufacturer { get; set; }

        [NotMapped] 
        private RetailingApp.Patterns.IProductState _state = new RetailingApp.Patterns.AvailableState();

        public void SetState(RetailingApp.Patterns.IProductState state)
        {
            _state = state;
        }

        public void Purchase(int amount)
        {
            _state.HandlePurchase(this, amount);
        }

        public string GetStatus() => _state.GetStatus();

        public Product() 
        {
            if (Quantity == 0) _state = new RetailingApp.Patterns.SoldOutState();
        }

        public Product(string name) 
        {
            Name = name;
        }
    }
}
