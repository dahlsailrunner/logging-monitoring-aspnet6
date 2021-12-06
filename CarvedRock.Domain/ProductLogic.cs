using CarvedRock.Domain.Models;
using Microsoft.Extensions.Logging;

namespace CarvedRock.Domain;

public class ProductLogic : IProductLogic
{
    private readonly ILogger<ProductLogic> _logger;

    public ProductLogic(ILogger<ProductLogic> logger)
    {
        _logger = logger;
    }
    public Task<IEnumerable<ProductModel>> GetProductsForCategory(string category)
    {
        _logger.LogInformation("Getting products in logic for {category}", category);
        
        return Task.FromResult(GetAllProducts().Where(a =>
               string.Equals("all", category, StringComparison.InvariantCultureIgnoreCase) ||
               string.Equals(category, a.Category, StringComparison.InvariantCultureIgnoreCase)));
    }

    private static IEnumerable<ProductModel> GetAllProducts()
    {
        return new List<ProductModel>
        {
            new ProductModel { Id = 1, Name = "Trailblazer", Category = "boots", Price = 69.99,
                Description = "Great support in this high-top to take you to great heights and trails." },
            new ProductModel { Id = 2, Name = "Coastliner", Category = "boots", Price = 49.99,
                Description = "Easy in and out with this lightweight but rugged shoe with great ventilation to get your around shores, beaches, and boats." },
            new ProductModel { Id = 3, Name = "Woodsman", Category = "boots", Price = 64.99,
                Description = "All the insulation and support you need when wandering the rugged trails of the woods and backcountry."},
            new ProductModel { Id = 4, Name = "Billy", Category = "boots", Price = 79.99,
                Description = "Get up and down rocky terrain like a billy-goat with these awesome high-top boots with outstanding support." },
        };
    }
}