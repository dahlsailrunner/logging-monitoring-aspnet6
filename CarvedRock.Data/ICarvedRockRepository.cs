using CarvedRock.Data.Entities;

namespace CarvedRock.Data
{
    public interface ICarvedRockRepository
    {
        Task<List<Product>> GetProductsAsync(string category);
        Task<Product?> GetProductByIdAsync(int id);

        List<Product> GetProducts(string category);
        Product? GetProductById(int id);
    }
}
