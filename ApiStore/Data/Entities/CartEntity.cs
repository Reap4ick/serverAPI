using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStore.Data.Entities
{
    [Table("tblCarts")]
    public class CartEntity
    {
        [Key]
        public int Id { get; set; } // ID кошика

        [Required]
        public int ProductId { get; set; } // ID продукту

        [Required]
        public int Quantity { get; set; } // Кількість продукту

        [Required]
        public string UserEmail { get; set; } = string.Empty; // Email користувача
    }
}
