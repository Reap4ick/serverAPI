using ApiStore.Data.Entities;
using ApiStore.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiStore.Data
{
    public class ApiStoreDbContext : IdentityDbContext<UserEntity, RoleEntity, int>
    {
        public ApiStoreDbContext(DbContextOptions<ApiStoreDbContext> options)
            : base(options) { }

        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductImages { get; set; }
        public DbSet<CartEntity> Carts { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderProductEntity> OrderProducts { get; set; }  // Додано зв'язок між замовленням і продуктами

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRoleEntity>(ur =>
            {
                ur.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(r => r.RoleId)
                    .IsRequired();

                ur.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(u => u.UserId)
                    .IsRequired();
            });

            builder.Entity<CartEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UserEmail).IsRequired();
            });

            builder.Entity<OrderEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderDate).IsRequired();
                entity.Property(e => e.UserEmail).IsRequired();
                entity.Property(e => e.TotalAmount).IsRequired();
            });

            // Налаштування для OrderProductEntity
            builder.Entity<OrderProductEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderProducts)
                      .HasForeignKey(e => e.OrderId);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId);
            });
        }
    }
}
