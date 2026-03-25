using PersonalLifeManager.Repositories;

namespace PersonalLifeManager.Services;

public class RefreshTokenService(IRefreshTokenRepository repo, ITokenService tokenService) : IRefreshTokenService
{
    public async Task<string> CreateAsync(string userId)
    {
        var refreshToken = tokenService.CreateRefreshToken();

        await repo.SaveAsync(userId, refreshToken, DateTime.UtcNow.AddDays(7));

        return refreshToken;
    }

    public async Task<string> ValidateAndGetUserIdAsync(string refreshToken)
    {
        var stored = await repo.ValidateAsync(refreshToken);

        if (stored == null)
            throw new UnauthorizedAccessException("Refresh token invalid or expired");

        return stored.UserId;
    }

    public async Task<string> RotateAsync(string userId, string oldToken)
    {
        await repo.RevokeAsync(oldToken);

        var newToken = tokenService.CreateRefreshToken();

        await repo.SaveAsync(userId, newToken, DateTime.UtcNow.AddDays(7));

        return newToken;
    }

    public async Task RevokeAsync(string refreshToken)
    {
        await repo.RevokeAsync(refreshToken);
    }
}