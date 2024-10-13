using Microsoft.AspNetCore.Mvc;
using ApiStore.Data;
using ApiStore.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using ApiStore.Models.Order;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ApiStoreDbContext _context;

    public OrderController(ApiStoreDbContext context)
    {
        _context = context;
    }

    // Метод для створення замовлення
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder()
    {
        var email = GetUserEmailFromToken();

        var userCartItems = _context.Carts.Where(c => c.UserEmail == email).ToList();
        if (!userCartItems.Any())
        {
            return BadRequest("Ваш кошик порожній.");
        }

        decimal totalAmount = 0;
        var orderProducts = new List<OrderProductEntity>();

        foreach (var item in userCartItems)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                totalAmount += product.Price * item.Quantity;

                orderProducts.Add(new OrderProductEntity
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity
                });
            }
        }

        var newOrder = new OrderEntity
        {
            OrderDate = DateTime.UtcNow,
            UserEmail = email,
            TotalAmount = totalAmount,
            OrderProducts = orderProducts
        };

        await _context.Orders.AddAsync(newOrder);
        _context.Carts.RemoveRange(userCartItems);

        await _context.SaveChangesAsync();

        return Ok("Замовлення успішно створено.");
    }

    // Метод для перегляду історії замовлень
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetUserOrders()
    {
        var email = GetUserEmailFromToken();
        var orders = await _context.Orders
            .Where(o => o.UserEmail == email)
            .Include(o => o.OrderProducts) // Завантажуємо продукти для кожного замовлення
            .ThenInclude(op => op.Product) // Завантажуємо інформацію про продукти
            .ToListAsync();

        if (orders == null || !orders.Any())
        {
            return NotFound("У вас немає замовлень.");
        }

        var orderDtos = orders.Select(order => new OrderDto
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Products = order.OrderProducts.Select(orderProduct => new ProductDto
            {
                ProductId = orderProduct.ProductId,
                ProductName = orderProduct.Product.Name,
                Quantity = orderProduct.Quantity
            }).ToList()
        }).ToList();

        return Ok(orderDtos);
    }

    private string GetUserEmailFromToken()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var token = authHeader.Substring("Bearer ".Length).Trim();
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
    }
}


