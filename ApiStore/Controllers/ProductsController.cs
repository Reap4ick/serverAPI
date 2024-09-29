using ApiStore.Data;
using ApiStore.Data.Entities;
using ApiStore.Interfaces;
using ApiStore.Models.Products;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ApiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApiStoreDbContext _context;
        private readonly IImageHulk _imageHulk;
        private readonly IMapper _mapper;

        public ProductsController(
            ApiStoreDbContext context,
            IImageHulk imageHulk,
            IMapper mapper)
        {
            _context = context;
            _imageHulk = imageHulk;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var list = _context.Products
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateViewModel model)
        {
            var productEntity = _mapper.Map<ProductEntity>(model);
            productEntity.ProductImages = new List<ProductImageEntity>();

            foreach (var image in model.Images)
            {
                var imageName = await _imageHulk.Save(image);
                productEntity.ProductImages.Add(new ProductImageEntity
                {
                    Image = imageName,
                    ProductId = productEntity.Id
                });
            }

            _context.Products.Add(productEntity);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Новий метод для видалення продуктів
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Виклик асинхронного видалення зображень
            foreach (var image in product.ProductImages)
            {
                await _imageHulk.DeleteAsync(image.Image);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("{id}")]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Return validation errors

            var product = await _context.Products
                .Include(p => p.ProductImages) // Include product images
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            _mapper.Map(model, product); // Map updated values

            // Handle new images
            if (model.NewImages != null)
            {
                foreach (var img in model.NewImages)
                {
                    if (img.Length > 0)
                    {
                        string imageName = await _imageHulk.Save(img);
                        var imgEntity = new ProductImageEntity
                        {
                            Image = imageName,
                            ProductId = product.Id
                        };
                        _context.ProductImages.Add(imgEntity);
                    }
                }
            }

            // Handle deleted images
            if (model.DeletedPhotoIds != null)
            {
                var photos = _context.ProductImages
                    .Where(pi => model.DeletedPhotoIds.Contains(pi.Id))
                    .ToList();

                _context.ProductImages.RemoveRange(photos);

                foreach (var photo in photos)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "images", photo.Image);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }

            await _context.SaveChangesAsync(); // Save changes

            return NoContent(); // Return 204 No Content on success
        }





    }
}
