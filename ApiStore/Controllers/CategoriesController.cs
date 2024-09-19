using ApiStore.Data;
using ApiStore.Data.Entities;
using ApiStore.Models.Category;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace ApiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApiStoreDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public CategoriesController(ApiStoreDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        // Get the list of categories
        [HttpGet]
        public IActionResult GetList()
        {
            var list = _context.Categories.ToList();
            return Ok(list);
        }

        // Create a new category
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string imageName = Guid.NewGuid().ToString() + ".webp";
            var dir = _configuration["ImagesDir"];

            using (MemoryStream ms = new())
            {
                await model.Image.CopyToAsync(ms);
                var bytes = ms.ToArray();
                int[] sizes = { 50, 150, 300, 600, 1200 };
                foreach (var size in sizes)
                {
                    string dirSave = Path.Combine(Directory.GetCurrentDirectory(), dir, $"{size}_{imageName}");

                    using (var image = Image.Load(bytes))
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(size, size),
                            Mode = ResizeMode.Max
                        }));

                        image.Save(dirSave, new WebpEncoder());
                    }
                }
            }

            var entity = _mapper.Map<CategoryEntity>(model);
            entity.Image = imageName;
            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Get category for editing
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryEditViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Image = c.Image
                })
                .FirstOrDefault();

            if (item == null)
                return NotFound(new { message = $"Category with id={id} not found" });

            return Ok(item);
        }

        // Edit category
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] CategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var item = _context.Categories.Find(id);
            if (item == null)
                return NotFound(new { message = "Category not found" });

            item.Name = model.Name;
            item.Description = model.Description;

            var dir = _configuration["ImagesDir"];
            var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), dir);

            // Перевірка наявності нового зображення
            if (model.NewImage != null && model.NewImage.Length > 0)
            {
                // Видалення старих зображень, якщо вони існують
                if (!string.IsNullOrEmpty(item.Image))
                {
                    int[] sizes = { 50, 150, 300, 600, 1200 };
                    foreach (var size in sizes)
                    {
                        var oldImagePath = Path.Combine(imagesDir, $"{size}_{item.Image}");
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }
                }

                // Генерація нового імені для зображення
                string newImageName = Guid.NewGuid().ToString() + ".webp";
                using (MemoryStream ms = new())
                {
                    await model.NewImage.CopyToAsync(ms);
                    var bytes = ms.ToArray();

                    // Збереження нових зображень у різних розмірах
                    int[] sizes = { 50, 150, 300, 600, 1200 };
                    foreach (var size in sizes)
                    {
                        string newImagePath = Path.Combine(imagesDir, $"{size}_{newImageName}");
                        using (var image = Image.Load(bytes))
                        {
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(size, size),
                                Mode = ResizeMode.Max
                            }));
                            image.Save(newImagePath, new WebpEncoder());
                        }
                    }
                }

                // Оновлення зображення в базі даних
                item.Image = newImageName;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }



        // Delete category
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _context.Categories.Find(id);
            if (item == null)
                return NotFound(new { message = "Category not found" });

            if (!string.IsNullOrEmpty(item.Image))
            {
                var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), _configuration["ImagesDir"]);
                int[] sizes = { 50, 150, 300, 600, 1200 };
                foreach (var size in sizes)
                {
                    var imagePath = Path.Combine(imagesDir, $"{size}_{item.Image}");
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }
            }

            _context.Categories.Remove(item);
            _context.SaveChanges();

            return Ok();
        }
    }
}
