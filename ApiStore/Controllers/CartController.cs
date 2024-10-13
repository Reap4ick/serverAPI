using Microsoft.AspNetCore.Mvc;
using ApiStore.Data;
using ApiStore.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using ApiStore.Models.Cart;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ApiStoreDbContext _context;

    public CartController(ApiStoreDbContext context)
    {
        _context = context;
    }

    // Метод для додавання продуктів до кошика
    [HttpPost("add")]
    public async Task<IActionResult> AddProductsToCart([FromBody] CartItemDto[] cartItems)
    {
        if (cartItems == null || !cartItems.Any())
        {
            return BadRequest("Список продуктів не може бути пустим.");
        }

        var email = GetUserEmailFromToken();

        foreach (var item in cartItems)
        {
            var cartEntity = new CartEntity
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UserEmail = email
            };
            await _context.Carts.AddAsync(cartEntity);
        }

        await _context.SaveChangesAsync();
        return Ok("Продукти успішно додано до кошика.");
    }

    // Метод для виведення товарів із кошика
    [HttpGet("items")]
    public IActionResult GetCartItems()
    {
        var email = GetUserEmailFromToken();

        // Отримуємо товари з кошика для користувача
        var userCartItems = _context.Carts
            .Where(c => c.UserEmail == email)
            .ToList();

        var result = userCartItems.Select(c => {
            var product = _context.Products.FirstOrDefault(p => p.Id == c.ProductId);
            return new
            {
                c.ProductId,
                c.Quantity,
                ProductName = product != null ? product.Name : "Продукт не знайдено",
                ProductPrice = product != null ? product.Price : 0
            };
        }).ToList();

        if (!result.Any())
        {
            return NotFound("Ваш кошик порожній.");
        }

        return Ok(result);
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

