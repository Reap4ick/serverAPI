using ApiStore.Data.Entities.Identity;

public interface IJwtTokenService
{
    Task<string> CreateTokenAsync(UserEntity user);
}