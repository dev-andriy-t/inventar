using System;
using System.Collections.Generic;
using System.Linq;
using RetailingApp.Entities;

namespace RetailingApp.Patterns
{
    public interface IProductSortStrategy
    {
        IEnumerable<Product> Sort(IEnumerable<Product> products);
    }

    public class SortByPriceAscendingStrategy : IProductSortStrategy
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderBy(p => p.Price);
        }
    }

    public class SortByNameStrategy : IProductSortStrategy
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderBy(p => p.Name);
        }
    }

    public class SortByCategory : IProductSortStrategy
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderBy(p => p.Categories.FirstOrDefault()?.Name);
        }
    }
}
