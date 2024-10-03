using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiStore.Models.Products
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Назва продукту є обов'язковою.")]
        [StringLength(255, ErrorMessage = "Назва продукту не повинна перевищувати 255 символів.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ціна продукту є обов'язковою.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більшою за нуль.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Виберіть категорію для продукту.")]
        public int CategoryId { get; set; }  // ID категорії продукту

        [BindProperty(Name = "images[]")]
        [Required(ErrorMessage = "Будь ласка, завантажте хоча б одне зображення.")]
        [MaxLength(5, ErrorMessage = "Можна завантажити не більше 5 зображень.")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
