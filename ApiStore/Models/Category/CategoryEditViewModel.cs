using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ApiStore.Models.Category
{
    public class CategoryEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва категорії є обов'язковою")]
        [StringLength(255, ErrorMessage = "Назва не може перевищувати 255 символів")]
        public string Name { get; set; }

        [StringLength(4000, ErrorMessage = "Опис не може перевищувати 4000 символів")]
        public string? Description { get; set; }  // Опис категорії

        public string? Image { get; set; }  // Поточне зображення

        // Нове зображення, яке користувач може завантажити
        public IFormFile? NewImage { get; set; }
    }
}

