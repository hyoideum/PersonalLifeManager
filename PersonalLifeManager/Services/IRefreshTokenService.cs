namespace PersonalLifeManager.Services;

public interface IRefreshTokenService
{
    Task<string> CreateAsync(string userId);
    Task<string> ValidateAndGetUserIdAsync(string refreshToken);
    Task<string> RotateAsync(string userId, string oldToken);
    Task RevokeAsync(string refreshToken);
}