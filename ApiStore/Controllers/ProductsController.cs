using ApiStore.Data;
using ApiStore.Data.Entities;
using ApiStore.Interfaces;
using ApiStore.Models.Category;

/*using ApiStore.Models.Product;*/
using ApiStore.Models.Products;
using ApiStore.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(
        ApiStoreDbContext context, IImageHulk imageHulk,
        IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetList()
        {
            var list = context.Products
                .ProjectTo<ProductItemViewModel>(mapper.ConfigurationProvider)
                .ToList();
            return Ok(list);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateViewModel model)
        {
            var entity = mapper.Map<ProductEntity>(model);
            context.Products.Add(entity);
            context.SaveChanges();

            if (model.Images != null)
            {
                var p = 1;
                foreach (var image in model.Images)
                {
                    var pi = new ProductImageEntity
                    {
                        Image = await imageHulk.Save(image),
                        Priority = p,
                        ProductId = entity.Id
                    };
                    p++;
                    context.ProductImages.Add(pi);
                    await context.SaveChangesAsync();
                }
            }
            return Created();
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Edit(int id, [FromForm] ProductEditViewModel model)

        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Завантажуємо продукт із зображеннями для редагування
            var product = await context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == model.Id)
                ?? throw new Exception("No product was found");

            // Мапінг нових даних на продукт
            mapper.Map(model, product);

            // Обробка нових зображень
            if (model.NewImages != null && model.NewImages.Any())
            {
                foreach (var img in model.NewImages)
                {
                    if (img.Length > 0)
                    {
                        // Збереження зображення в різних розмірах і форматі .webp
                        string fname = await imageHulk.Save(img);
                        var imgEntity = new ProductImageEntity
                        {
                            Image = fname,
                            Product = product
                        };
                        context.ProductImages.Add(imgEntity);
                    }
                }
            }

            // Обробка видалених зображень
            if (model.DeletedPhotoIds != null)
            {
                var photos = context.ProductImages
                    .Where(pi => model.DeletedPhotoIds.Contains(pi.Id))
                    .ToList();

                context.ProductImages.RemoveRange(photos);

                foreach (var photo in photos)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "images", photo.Image);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }

            // Збереження змін у базі
            await context.SaveChangesAsync();

            return Ok();
        }




        /*[HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = context.Products
                .Include(p => p.ProductImages)
                .ProjectTo<ProductEditViewModel>(mapper.ConfigurationProvider)
                .SingleOrDefault(x => x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }*/

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = context.Products
                .ProjectTo<ProductItemViewModel>(mapper.ConfigurationProvider)
                .SingleOrDefault(x => x.Id == id);
            return Ok(item);
        }



        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = context.Products
                .Include(x=>x.ProductImages)
                .SingleOrDefault(x => x.Id == id);

            if (product == null) return NotFound();

            if (product.ProductImages != null)
                foreach (var p in product.ProductImages)
                    imageHulk.Delete(p.Image);

            context.Products.Remove(product);
            context.SaveChanges();
            return Ok();
        }
    }
}



/*using ApiStore.Data;
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
*/