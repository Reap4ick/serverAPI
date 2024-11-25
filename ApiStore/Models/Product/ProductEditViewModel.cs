using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using ApiStore.Models.Products;

namespace ApiStore.Models.Products
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
/*        [RegularExpression(@"^\d+([\,\.]\d{1,})?$", ErrorMessage = "Provide valid price")]*/
        public string? Price { get; set; }
        public SelectList? CategoryList { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Choose a category")]
        public int CategoryId { get; set; }
        public List<ProductImageViewModel>? Images { get; set; }

        [Display(Name = "New images")]
        public List<IFormFile>? NewImages { get; set; }

        [Display(Name = "Delete image")]
        public List<int>? DeletedPhotoIds { get; set; }

        public List<string>? DeletedPhotoNames { get; set; } // Імена файлів для видалення

        public string Description { get; set; } = String.Empty;

/*        public IEnumerable<string> DeletedDescImageNames { get; set; }

        public IEnumerable<IFormFile> NewDescImages { get; set; }*/

    }
}

/*
﻿using Microsoft.AspNetCore.Mvc;

namespace ApiStore.Models.Product
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        //public List<string>? RemoveImages { get; set; }
        [BindProperty(Name = "images[]")]
        public List<IFormFile>? Images { get; set; }
    }
}*/