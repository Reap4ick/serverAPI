using ApiStore.Data.Entities.Identity;
using ApiStore.Interfaces;
using ApiStore.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AccountController(UserManager<UserEntity> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return BadRequest("Не вірно вказано дані");

                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                    return BadRequest("Не вірно вказано дані");

                var token = await _jwtTokenService.CreateTokenAsync(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Створення нового користувача
                var user = new UserEntity
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname
                };

                // Спроба створення користувача з вказаним паролем
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                // Призначення ролі користувачу
                await _userManager.AddToRoleAsync(user, "User");

                // Генерація JWT токену
                var token = await _jwtTokenService.CreateTokenAsync(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Сталася помилка при реєстрації: {ex.Message}");
            }
        }


        // Метод для отримання профілю користувача
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            try
            {
                // Отримуємо заголовок Authorization
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                    return Unauthorized("Необхідно надати JWT токен");

                // Витягуємо сам токен
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Парсимо токен
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                if (jwtToken == null)
                    return Unauthorized("Невірний токен");

                // Витягуємо клейми з токена (Payload)
                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                var roles = jwtToken.Claims.FirstOrDefault(c => c.Type == "roles")?.Value;
                var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

                // Повертаємо інформацію з токена
                return Ok(new
                {
                    email,
                    name,
                    roles,
                    exp
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // Метод для логауту
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                // Тут можна додати логику для очищення JWT токенів з клієнтської сторони
                return Ok("Ви успішно вийшли з системи");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
