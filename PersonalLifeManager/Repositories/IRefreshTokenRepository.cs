namespace PersonalLifeManager.Repositories;

public interface IRefreshTokenRepository
{
    Task SaveAsync(string userId, string token, DateTime expiry);
    Task<RefreshToken?> ValidateAsync(string token);
    Task RevokeAsync(string token);
}