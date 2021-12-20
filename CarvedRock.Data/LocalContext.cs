using CarvedRock.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarvedRock.Data
{
    public class LocalContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;

        public string DbPath { get; set; }

        public LocalContext()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DbPath = Path.Join(path, "carvedrock-logging.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        public void MigrateAndCreateData()
        {
            Database.Migrate();

            if (Products.Any()) return;

            Products.Add(new Product
            {
                Name = "Trailblazer",
                Category = "boots",
                Price = 69.99,
                Description = "Great support in this high-top to take you to great heights and trails."
            });
            Products.Add(new Product
            {
                Name = "Coastliner",
                Category = "boots",
                Price = 49.99,
                Description =
                    "Easy in and out with this lightweight but rugged shoe with great ventilation to get your around shores, beaches, and boats."
            });
            Products.Add(new Product
            {
                Name = "Woodsman",
                Category = "boots",
                Price = 64.99,
                Description =
                    "All the insulation and support you need when wandering the rugged trails of the woods and backcountry."
            });
            Products.Add(new Product
            {
                Name = "Billy",
                Category = "boots",
                Price = 79.99,
                Description =
                    "Get up and down rocky terrain like a billy-goat with these awesome high-top boots with outstanding support."
            });

            SaveChanges();
        }
    }
}
