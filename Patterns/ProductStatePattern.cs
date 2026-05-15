using System;
using System;
using RetailingApp.Entities;


namespace RetailingApp.Patterns
{
    public interface IProductState{
        void HandlePurchase(Product product, int quantity);
        string GetStatus();
    }

    public class AvailableState: IProductState
    {
        public void HandlePurchase(Product product, int quantity)
        {
            if(quantity <= product.Quantity)
            {
                product.Quantity -= quantity;
                if(product.Quantity == 0)
                {
                    product.SetState(new SoldOutState());
                }
            } 
            else 
            {
                throw new InvalidOperationException("Not enough item in this stock"); 
            }
        }
        public string GetStatus() => "This is available item";
    }

    public class SoldOutState: IProductState
    {
        public void HandlePurchase(Product product, int quantity)
        {
            throw new InvalidOperationException("Item is unavailable so you can't buy it");
        }

        public string GetStatus() => "This is unavailable item";
    }
}
