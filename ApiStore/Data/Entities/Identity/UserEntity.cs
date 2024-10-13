using Microsoft.AspNetCore.Identity;
using System.Collections.Generic; // Додано для ICollection
using System.ComponentModel.DataAnnotations;

namespace ApiStore.Data.Entities.Identity
{
    public class UserEntity : IdentityUser<int>
    {
        [StringLength(255)]
        public string? Image { get; set; }

        [StringLength(100)]
        public string? Firstname { get; set; }

        [StringLength(100)]
        public string? Lastname { get; set; }

        // Додано колекцію для кошиків
        public virtual ICollection<CartEntity> Carts { get; set; } = new List<CartEntity>();

        public virtual ICollection<UserRoleEntity>? UserRoles { get; set; }
    }
}
