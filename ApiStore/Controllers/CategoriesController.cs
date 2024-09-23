using ApiStore.Data;
using ApiStore.Data.Entities;
using ApiStore.Interfaces;
using ApiStore.Models.Category;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(
        ApiStoreDbContext context, IImageHulk imageHulk,
        IMapper mapper) : ControllerBase
    {
        public IImageHulk ImageHulk { get; } = imageHulk;

        // Get the list of categories
        [HttpGet]
        public IActionResult GetList()
        {
            var list = context.Categories.ToList();
            return Ok(list);
        }

        // Create a new category
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var imageName = await ImageHulk.Save(model.Image);
            var entity = mapper.Map<CategoryEntity>(model);
            entity.Image = imageName;
            context.Categories.Add(entity);
            await context.SaveChangesAsync();

            return Ok();
        }

        // Get category by id
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = context.Categories
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

            var item = context.Categories.Find(id);
            if (item == null)
                return NotFound(new { message = "Category not found" });

            item.Name = model.Name;
            item.Description = model.Description;

            // Check if a new image is provided
            if (model.NewImage != null && model.NewImage.Length > 0)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(item.Image))
                    ImageHulk.Delete(item.Image);

                // Save new image
                var newImageName = await ImageHulk.Save(model.NewImage);
                item.Image = newImageName;
            }

            await context.SaveChangesAsync();
            return Ok();
        }

        // Delete category
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = context.Categories.Find(id);
            if (item == null)
                return NotFound(new { message = "Category not found" });

            if (!string.IsNullOrEmpty(item.Image))
                ImageHulk.Delete(item.Image);

            context.Categories.Remove(item);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
