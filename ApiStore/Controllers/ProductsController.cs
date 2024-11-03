using ApiStore.Data;
using ApiStore.Data.Entities;
using ApiStore.Interfaces;
using ApiStore.Models.Products;
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
            await _context.SaveChangesAsync();

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
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Products
                .Include(p => p.ProductImages)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (product == null) return NotFound();

            if (product.ProductImages != null)
            {
                foreach (var p in product.ProductImages)
                {
                    await _imageHulk.DeleteAsync(p.Image);
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
