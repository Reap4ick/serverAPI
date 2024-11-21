using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStore.Data.Entities
{
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; } = String.Empty;
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public CategoryEntity? Category { get; set; }
        public virtual ICollection<ProductImageEntity>? ProductImages { get; set; }
        public virtual ICollection<ProductDescImageEntity>? ProductDescImages { get; set; }
    }
}