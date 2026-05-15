using System;
using Moq;
using Xunit;
using RetailingApp.Services;
using RetailingApp.Entities;
using RetailingApp.Repositories;
using RetailingApp.Patterns;

namespace RetailingApp.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IRepository<Manufacturer>> _mockManufacturerRepo;
        private readonly Mock<IRepository<Category>> _mockCategoryRepo;
        private readonly Mock<IRepository<Location>> _mockLocationRepo;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepo = new Mock<IProductRepository>();
            _mockManufacturerRepo = new Mock<IRepository<Manufacturer>>();
            _mockCategoryRepo = new Mock<IRepository<Category>>();
            _mockLocationRepo = new Mock<IRepository<Location>>();

            _productService = new ProductService(
                _mockProductRepo.Object,
                _mockManufacturerRepo.Object,
                _mockCategoryRepo.Object,
                _mockLocationRepo.Object
            );
        }

        [Fact]
        public void GetProductPrice_BlackFridayStrategy_ReturnsCorrectDiscount()
        {
            var product = new Product { Id = 1, Name = "Laptop", Price = 1000 };
            _mockProductRepo.Setup(repo => repo.GetById(1)).Returns(product);

            var result = _productService.GetProductPrice(1, "blackfriday");

            Assert.Equal(1000, result.Original);
            Assert.Equal(700, result.Final);
            Assert.Equal("BlackFridayStategy", result.Strategy);
        }

        [Fact]
        public void GetProductPrice_UnknownStrategy_ReturnsOriginalPrice()
        {
            var product = new Product { Id = 2, Name = "Phone", Price = 500 };
            _mockProductRepo.Setup(repo => repo.GetById(2)).Returns(product);

            var result = _productService.GetProductPrice(2, "unknown");

            Assert.Equal(500, result.Original);
            Assert.Equal(500, result.Final);
            Assert.Equal("NoDiscountStrategy", result.Strategy);
        }

        [Fact]
        public void GetProductPrice_ProductNotFound_ThrowsException()
        {
            _mockProductRepo.Setup(repo => repo.GetById(99)).Returns((Product?)null);

            var ex = Assert.Throws<Exception>(() => _productService.GetProductPrice(99, "none"));
            Assert.Equal("Product not found", ex.Message);
        }

        [Fact]
        public void PurchaseProduct_AvailableState_ReducesQuantity()
        {
            var product = new Product { Id = 3, Name = "Tablet", Quantity = 10 };
            product.SetState(new AvailableState());
            _mockProductRepo.Setup(repo => repo.GetById(3)).Returns(product);

            var purchased = _productService.PurchaseProduct(3, 2);

            Assert.NotNull(purchased);
            Assert.Equal(8, purchased.Quantity);
            Assert.Equal("This is available item", purchased.GetStatus());
            _mockProductRepo.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public void PurchaseProduct_AvailableStateToSoldOut_ChangesState()
        {
            var product = new Product { Id = 4, Name = "Monitor", Quantity = 2 };
            product.SetState(new AvailableState());
            _mockProductRepo.Setup(repo => repo.GetById(4)).Returns(product);

            var purchased = _productService.PurchaseProduct(4, 2);

            Assert.NotNull(purchased);
            Assert.Equal(0, purchased.Quantity);
            Assert.Equal("This is unavailable item", purchased.GetStatus());
        }

        [Fact]
        public void PurchaseProduct_NotEnoughQuantity_ThrowsException()
        {
            var product = new Product { Id = 5, Name = "Mouse", Quantity = 1 };
            product.SetState(new AvailableState());
            _mockProductRepo.Setup(repo => repo.GetById(5)).Returns(product);

            var ex = Assert.Throws<InvalidOperationException>(() => _productService.PurchaseProduct(5, 5));
            Assert.Equal("Not enough item in this stock", ex.Message);
            Assert.Equal(1, product.Quantity);
            _mockProductRepo.Verify(repo => repo.Save(), Times.Never);
        }
    }
}
