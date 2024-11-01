﻿namespace ApiStore.Models.Products
{
    public class ProductItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public List<string>? Images { get; set; }

    }
}