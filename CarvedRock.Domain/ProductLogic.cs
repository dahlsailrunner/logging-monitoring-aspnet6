using CarvedRock.Data;
using CarvedRock.Data.Entities;
using Microsoft.Extensions.Logging;

namespace CarvedRock.Domain;

public class ProductLogic : IProductLogic
{
    private readonly ILogger<ProductLogic> _logger;
    private readonly ICarvedRockRepository _repo;

    public ProductLogic(ILogger<ProductLogic> logger, ICarvedRockRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }
    public async Task<IEnumerable<Product>> GetProductsForCategoryAsync(string category)
    {
        _logger.LogInformation("Getting products in logic for {category}", category);

        return await _repo.GetProductsAsync(category);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _repo.GetProductByIdAsync(id);
    }

    public IEnumerable<Product> GetProductsForCategory(string category)
    {
        return _repo.GetProducts(category);
    }

    public Product? GetProductById(int id)
    {
        return _repo.GetProductById(id);
    }
}