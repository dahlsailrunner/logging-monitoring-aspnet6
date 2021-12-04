using CarvedRock.Domain.Models;

namespace CarvedRock.Domain;

public interface IProductLogic 
{
    Task<IEnumerable<ProductModel>> GetProductsForCategory(string category);
}