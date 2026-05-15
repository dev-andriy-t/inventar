using System;
using System.Collections.Generic;
using System.Linq;
using RetailingApp.Models;
using RetailingApp.Entities;
using RetailingApp.Patterns;
using RetailingApp.Repositories;

namespace RetailingApp.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetProductsByCategory(int categoryId);
        Product? CreateProduct(ProductDto dto);
        void UpdateProduct(int id, Product updatedProduct);
        void DeleteProduct(int id);
        Product? PurchaseProduct(int id, int amount);
        (double Original, double Final, string Strategy) GetProductPrice(int id, string strategyType);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IRepository<Manufacturer> _manufacturerRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Location> _locationRepo;

        public ProductService(
            IProductRepository productRepo,
            IRepository<Manufacturer> manufacturerRepo,
            IRepository<Category> categoryRepo,
            IRepository<Location> locationRepo)
        {
            _productRepo = productRepo;
            _manufacturerRepo = manufacturerRepo;
            _categoryRepo = categoryRepo;
            _locationRepo = locationRepo;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            AppLogger.Instance.LogInfo("Service: Fetching all products");
            return _productRepo.GetAllWithDetails();
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            AppLogger.Instance.LogInfo($"Service: Fetching products for category {categoryId}");
            return _productRepo.GetProductsByCategory(categoryId);
        }

        public Product? CreateProduct(ProductDto dto)
        {
            AppLogger.Instance.LogInfo($"Service: Creating new product {dto.Name}");

            var newProduct = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Quantity = dto.Quantity
            };

            if (dto.ManufacturerId.HasValue && dto.ManufacturerId.Value > 0)
            {
                var man = _manufacturerRepo.GetById(dto.ManufacturerId.Value);
                if (man != null) newProduct.Manufacturer = man;
            }

            if (dto.LocationId.HasValue && dto.LocationId.Value > 0)
            {
                var loc = _locationRepo.GetById(dto.LocationId.Value);
                if (loc != null) newProduct.Location = loc;
            }

            foreach (var categoryId in dto.CategoryIds)
            {
                var cat = _categoryRepo.GetById(categoryId);
                if (cat != null) newProduct.Categories.Add(cat);
            }

            _productRepo.Add(newProduct);
            _productRepo.Save();
            return newProduct;
        }

        public void UpdateProduct(int id, Product updatedProduct)
        {
            AppLogger.Instance.LogInfo($"Service: Updating product ID {id}");
            _productRepo.Update(updatedProduct);
            _productRepo.Save();
        }

        public void DeleteProduct(int id)
        {
            AppLogger.Instance.LogInfo($"Service: Deleting product ID {id}");
            _productRepo.Delete(id);
            _productRepo.Save();
        }

        public Product? PurchaseProduct(int id, int amount)
        {
            var product = _productRepo.GetById(id);
            if (product == null) return null;

            product.Purchase(amount);
            _productRepo.Save();
            return product;
        }

        public (double Original, double Final, string Strategy) GetProductPrice(int id, string strategyType)
        {
            var product = _productRepo.GetById(id);
            if (product == null) throw new Exception("Product not found");

            IDiscountStrategy strategy = strategyType.ToLower() switch
            {
                "half" => new HalfPriceStrategy(),
                "fixed" => new FixedAmountDiscountStrategy(10),
                "blackfriday" => new BlackFridayStategy(),
                _ => new NoDiscountStrategy()
            };

            double finalPrice = strategy.CalculateDiscountedPrice(product.Price);
            return (product.Price, finalPrice, strategy.GetType().Name);
        }
    }

    public interface ICrudService<T, TDto>
    {
        IEnumerable<T> GetAll();
        T Create(TDto dto);
        void Update(int id, T entity);
        void Delete(int id);
    }

    public class LocationService : ICrudService<Location, LocationDto>
    {
        private readonly IRepository<Location> _repo;
        public LocationService(IRepository<Location> repo) => _repo = repo;

        public IEnumerable<Location> GetAll() => _repo.GetAll();
        public Location Create(LocationDto dto)
        {
            var loc = new Location { Name = dto.Name };
            _repo.Add(loc);
            _repo.Save();
            return loc;
        }
        public void Update(int id, Location entity) { _repo.Update(entity); _repo.Save(); }
        public void Delete(int id) { _repo.Delete(id); _repo.Save(); }
    }
}
