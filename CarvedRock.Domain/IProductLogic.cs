using CarvedRock.Data.Entities;

namespace CarvedRock.Domain;

public interface IProductLogic 
{
    Task<IEnumerable<Product>> GetProductsForCategoryAsync(string category);
    Task<Product?> GetProductByIdAsync(int id);
    IEnumerable<Product> GetProductsForCategory(string category);
    Product? GetProductById(int id);
}