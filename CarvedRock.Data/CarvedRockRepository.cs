using CarvedRock.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarvedRock.Data
{
    public class CarvedRockRepository :ICarvedRockRepository
    {
        private readonly LocalContext _ctx;

        public CarvedRockRepository(LocalContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<List<Product>> GetProductsAsync(string category)
        {
            return await _ctx.Products.Where(p => p.Category == category || category == "all").ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _ctx.Products.FindAsync(id);
        }

        public List<Product> GetProducts(string category)
        {
            return _ctx.Products.Where(p => p.Category == category || category == "all").ToList();
        }

        public Product? GetProductById(int id)
        {
            return _ctx.Products.Find(id);
        }
    }
}
