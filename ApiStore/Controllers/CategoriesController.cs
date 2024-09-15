using ApiStore.Data;
using ApiStore.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ApiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApiStoreDbContext _context;

        public CategoriesController(ApiStoreDbContext context)
        {
            _context = context;
        }

        // Метод для створення нової категорії
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryEntity category)
        {
            // Перевірка валідності даних
            if (category == null || string.IsNullOrEmpty(category.Name))
            {
                return BadRequest("Невірні дані для категорії.");
            }

            // Додавання нової категорії до контексту
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Повертаємо успішну відповідь з новоствореною категорією
            return CreatedAtAction(nameof(GetList), new { id = category.Id }, category);
        }

        // Метод для отримання списку категорій
        [HttpGet]
        public IActionResult GetList()
        {
            var list = _context.Categories.ToList();
            return Ok(list);
        }
    }
}
