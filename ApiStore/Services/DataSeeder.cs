using ApiStore.Data;
using ApiStore.Data.Entities;
using ApiStore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiStore.Services
{
    public class DataSeeder
    {
        private readonly ApiStoreDbContext _context;
        private readonly IImageHulk _imageHulk;
        private readonly HttpClient _httpClient;

        public DataSeeder(ApiStoreDbContext context, IImageHulk imageHulk)
        {
            _context = context;
            _imageHulk = imageHulk;
            _httpClient = new HttpClient();
        }

        public async Task SeedAsync()
        {
            // Ensure database is created and migrated
            await _context.Database.MigrateAsync();

            // Check if any categories exist, and create a default category if necessary
            if (!_context.Categories.Any())
            {
                var category = new CategoryEntity
                {
                    Name = "Default Category",
                    Description = "This is a default category.",
                    Image = "default_category.jpg"
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync(); // Ensure that changes are saved in the database
            }

            // Retrieve the default category to assign to products
            var defaultCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Default Category");

            if (defaultCategory == null)
            {
                // Log the error and throw an exception if the category was not found
                throw new Exception("Default category was not created or found.");
            }

            // Seed products if not exist
            if (!_context.Products.Any())
            {
                var products = new List<ProductEntity>
                {
                    new ProductEntity
                    {
                        Name = "Product 1",
                        Price = 19.99m,
                        CategoryId = defaultCategory.Id, // Ensure valid CategoryId
                        ProductImages = new List<ProductImageEntity>
                        {
                            await CreateProductImageAsync("https://via.placeholder.com/1200")
                        }
                    },
                    new ProductEntity
                    {
                        Name = "Product 2",
                        Price = 29.99m,
                        CategoryId = defaultCategory.Id,
                        ProductImages = new List<ProductImageEntity>
                        {
                            await CreateProductImageAsync("https://via.placeholder.com/600")
                        }
                    }
                };

                _context.Products.AddRange(products);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<ProductImageEntity> CreateProductImageAsync(string imageUrl)
        {
            // Download image from the provided URL
            var response = await _httpClient.GetAsync(imageUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Image download failed: {response.StatusCode}");
            }

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                // Convert stream to IFormFile and save using ImageHulk service
                var imageName = await _imageHulk.Save(new FormFile(stream, 0, stream.Length, "image", Path.GetFileName(imageUrl)));
                return new ProductImageEntity
                {
                    Image = imageName
                };
            }
        }
    }
}
