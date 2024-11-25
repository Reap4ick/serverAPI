using ApiStore.Data;
using ApiStore.Data.Entities;
using ApiStore.Interfaces;
using ApiStore.Models.Product;
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
            var entity = _mapper.Map<ProductEntity>(model);
            _context.Products.Add(entity);
            _context.SaveChanges();

            if (model.ImagesDescIds.Any())
            {
                await _context.ProductDescImages
                    .Where(x => model.ImagesDescIds.Contains(x.Id))
                    .ForEachAsync(x => x.ProductId = entity.Id);
            }

            if (model.Images != null)
            {
                var p = 1;
                foreach (var image in model.Images)
                {
                    var pi = new ProductImageEntity
                    {
                        Image = await _imageHulk.Save(image),
                        Priority = p,
                        ProductId = entity.Id
                    };
                    p++;
                    _context.ProductImages.Add(pi);
                    await _context.SaveChangesAsync();
                }
            }
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductDescImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound("Product not found");

            // Копіюємо дані з моделі
            _mapper.Map(model, product);

            // Обробка нових зображень
            if (model.NewImages != null && model.NewImages.Any())
            {
                foreach (var img in model.NewImages)
                {
                    if (img.Length > 0)
                    {
                        string fname = await _imageHulk.Save(img);
                        var imgEntity = new ProductImageEntity
                        {
                            Image = fname,
                            ProductId = product.Id // Прив’язуємо нове зображення до продукту
                        };
                        _context.ProductImages.Add(imgEntity);
                    }
                }
            }

            // Обробка видалених зображень за ім'ям
            if (model.DeletedPhotoNames != null && model.DeletedPhotoNames.Any())
            {
                var photos = await _context.ProductImages
                    .Where(pi => model.DeletedPhotoNames.Contains(pi.Image))
                    .ToListAsync();

                _context.ProductImages.RemoveRange(photos);

                foreach (var photo in photos)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "images", photo.Image);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
            }


            // Перевірка наявності записів із ProductId = null в таблиці ProductDescImages
            var orphanedDescImages = await _context.ProductDescImages
                .Where(pdi => pdi.ProductId == null)
                .ToListAsync();

            if (orphanedDescImages.Any())
            {
                _context.ProductDescImages.RemoveRange(orphanedDescImages);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.Products
                .Include(p => p.ProductImages)
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .SingleOrDefault(x => x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Отримання продукту з пов'язаними зображеннями
                var product = await _context.Products
                    .Include(x => x.ProductImages)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                // Видалення зображень опису з таблиці ProductDescImages
                var descImages = await _context.ProductDescImages
                    .Where(x => x.ProductId == id)
                    .ToListAsync();

                foreach (var descImage in descImages)
                {
                    try
                    {
                        // Видалення фізичних файлів зображень
                        await _imageHulk.DeleteAsync(descImage.Image);

                        // Видалення запису з бази даних
                        _context.ProductDescImages.Remove(descImage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete description image: {descImage.Image}, Error: {ex.Message}");
                    }
                }

                // Видалення зображень продукту
                if (product.ProductImages != null)
                {
                    foreach (var p in product.ProductImages)
                    {
                        try
                        {
                            await _imageHulk.DeleteAsync(p.Image);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to delete product image: {p.Image}, Error: {ex.Message}");
                        }
                    }
                }

                // Видалення самого продукту
                _context.Products.Remove(product);

                // Збереження змін у базі даних
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Логування повної помилки
                Console.WriteLine($"Error while deleting product: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }*/



        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductDescImages)
                .SingleOrDefault(x => x.Id == id);
            if (product == null) return NotFound();

            if (product.ProductImages != null)
                foreach (var p in product.ProductImages)
                    _imageHulk.Delete(p.Image);

            if (product.ProductDescImages != null)
                foreach (var p in product.ProductDescImages)
                    _imageHulk.Delete(p.Image);

            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok();
        }


        /*
                [HttpPost("SaveDescription")]
                public async Task<IActionResult> SaveProductDescription([FromBody] ProductDescriptionViewModel model)
                {
                    // Знаходимо продукт за ID
                    var product = await _context.Products.FindAsync(model.ProductId);

                    if (product == null)
                        return NotFound("Product not found");   

                    // Оновлюємо опис
                    product.Description = model.Description;

                    // Зберігаємо зміни
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Description updated successfully" });
                }

        */
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDescImage([FromForm] ProductDescImageUploadViewModel model)
        {
            if (model.Image != null)
            {
                var pdi = new ProductDescImageEntity
                {
                    Image = await _imageHulk.Save(model.Image),
                    DateCreate = DateTime.Now.ToUniversalTime(),

                };
                _context.ProductDescImages.Add(pdi);
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<ProductDescImageIdViewModel>(pdi));
            }
            return BadRequest();
        }
    }

}
