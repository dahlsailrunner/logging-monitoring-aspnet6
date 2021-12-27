using System.Diagnostics;
using CarvedRock.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarvedRock.Data
{
    public class CarvedRockRepository :ICarvedRockRepository
    {
        private readonly LocalContext _ctx;
        private readonly ILogger<CarvedRockRepository> _logger;        
        private readonly ILogger _factoryLogger;

        public CarvedRockRepository(LocalContext ctx, ILogger<CarvedRockRepository> logger,
            ILoggerFactory loggerFactory)
        {
            _ctx = ctx;
            _logger = logger;
            _factoryLogger = loggerFactory.CreateLogger("DataAccessLayer");
        }
        public async Task<List<Product>> GetProductsAsync(string category)
        {
            _logger.LogInformation("Getting products in repository for {category}", category);
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
            var timer = new Stopwatch();  
            timer.Start();          
            var product = _ctx.Products.Find(id);
            timer.Stop();

            _logger.LogDebug("Querying products for {id} finished in {milliseconds} milliseconds", 
                id, timer.ElapsedMilliseconds);	 

            _factoryLogger.LogInformation("(F) Querying products for {id} finished in {ticks} ticks", 
                id, timer.ElapsedTicks);           

            return product;
        }       
    }
}
