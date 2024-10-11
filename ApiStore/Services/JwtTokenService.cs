using ApiStore.Data.Entities.Identity;
using ApiStore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiStore.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserEntity> _userManager;

        public JwtTokenService(IConfiguration configuration, UserManager<UserEntity> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim("email", user.Email),
                new Claim("name", $"{user.Lastname} {user.Firstname}")
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            var key = Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"]);
            var signinKey = new SymmetricSecurityKey(key);
            var signinCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: signinCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
