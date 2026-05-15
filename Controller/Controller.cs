using Microsoft.AspNetCore.Mvc;
using RetailingApp.Models;
using RetailingApp.Patterns;
using RetailingApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using RetailingApp.Entities;
    
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase 
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService) => _productService = productService;

    [HttpGet] 
    public ActionResult<IEnumerable<Product>> GetProducts() 
    {
        return Ok(_productService.GetAllProducts());
    }

    [HttpPost]
    public ActionResult<Product> createProduct(ProductDto dto)
    {
        var newProduct = _productService.CreateProduct(dto);
        if (newProduct == null) return BadRequest("Failed to create product");
        return CreatedAtAction(nameof(GetProducts), new { id = newProduct.Id }, newProduct);
    }
    
    [HttpDelete("{id}")]
    public ActionResult deleteProduct(int id)
    {
        _productService.DeleteProduct(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public ActionResult updateProduct(int id, Product updatedProduct)
    {
        if (id != updatedProduct.Id) return BadRequest();
        _productService.UpdateProduct(id, updatedProduct);
        return NoContent();
    }

    [HttpPost("{id}/purchase")]
    public ActionResult PurchaseProduct(int id, [FromQuery] int amount = 1)
    {
        try
        {
            var product = _productService.PurchaseProduct(id, amount);
            if (product == null) return NotFound();
            return Ok(new { Message = $"Purchase successful. Status: {product.GetStatus()}", Remaining = product.Quantity });
        }
        catch (InvalidOperationException ex)
        {
            AppLogger.Instance.LogError($"Purchase failed: {ex.Message}");
            return BadRequest(new { Error = ex.Message });
        }
    }



    [HttpGet("{id}/price")]
    public ActionResult GetDiscountedPrice(int id, [FromQuery] string strategyType = "none")
    {
        try 
        {
            var result = _productService.GetProductPrice(id, strategyType);
            return Ok(new { OriginalPrice = result.Original, FinalPrice = result.Final, StrategyUsed = result.Strategy });
        }
        catch(Exception)
        {
            return NotFound();
        }
    }

    [HttpGet("category/{categoryId}")]
    public ActionResult<IEnumerable<Product>> GetProductsByCategory(int categoryId)
    {
        var products = _productService.GetProductsByCategory(categoryId);
        return Ok(products);
    }
}

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly ICrudService<Location, LocationDto> _locationService;
    public LocationController(ICrudService<Location, LocationDto> locationService) => _locationService = locationService;

    [HttpGet]
    public ActionResult<IEnumerable<Location>> Get() => Ok(_locationService.GetAll());

    [HttpPost]
    public ActionResult<Location> Create(LocationDto dto)
    {
        var loc = _locationService.Create(dto);
        return CreatedAtAction(nameof(Get), new { id = loc.Id }, loc);
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, Location loc)
    {
        if (id != loc.Id) return BadRequest();
        _locationService.Update(id, loc);
        return NoContent();
    }




    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        _locationService.Delete(id);
        return NoContent();
    }
}

[Route("api/[controller]")]
[ApiController]

public class CategoryController : ControllerBase{
    private readonly AppDbContext _context;
    public CategoryController(AppDbContext context) => _context = context;

    [HttpGet]
    public ActionResult<IEnumerable<Category>> Get()
    {
        return _context.Categories.ToList();
    }

    
    [HttpPost]
    public ActionResult<Category> CreateCategory(Category newCategory){
        _context.Categories.Add(newCategory);
        _context.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, newCategory);
    }

    [HttpPut("{id}")]
    public ActionResult<Category> UpdateCategory(int id, Category updatedCategory){
        if(id != updatedCategory.Id){
            return BadRequest();
        }
        _context.Categories.Update(updatedCategory);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult<Category> DeleteCategory(int id){
        var category = _context.Categories.Find(id);
        if (category == null){
            return NotFound();
        }
        _context.Categories.Remove(category);
        _context.SaveChanges();
        return NoContent();
    }
}

[Route("api/[controller]")]
[ApiController]
public class ManufacturerController : ControllerBase{
    private readonly AppDbContext _context;
    public ManufacturerController(AppDbContext context) => _context = context;

    [HttpGet]
    public ActionResult<IEnumerable<Manufacturer>> Get()
    {
        return _context.Manufacturers.ToList();
    }
    
    [HttpPost]
    public ActionResult<Manufacturer> Create(Manufacturer newManufacturer){
        _context.Manufacturers.Add(newManufacturer);
        _context.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = newManufacturer.Id }, newManufacturer);
    }

    [HttpDelete("{id}")]
    public ActionResult<Manufacturer> Delete(int id){
        var manufacturer = _context.Manufacturers.Find(id);
        if(manufacturer == null){
            return NotFound();
        }
        _context.Manufacturers.Remove(manufacturer);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPut("{id}")]
    public ActionResult<Manufacturer> Update(int id, Manufacturer updatedManufacturer){
        if(id != updatedManufacturer.Id){
            return BadRequest();
        }
        _context.Manufacturers.Update(updatedManufacturer);
        _context.SaveChanges();
        return NoContent();
    }

}