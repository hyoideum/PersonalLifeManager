using Microsoft.EntityFrameworkCore;
using PersonalLifeManager.Data;

namespace PersonalLifeManager.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task SaveAsync(string userId, string token, DateTime expiry)
    {
        var oldTokens = context.RefreshTokens.Where(t => t.UserId == userId);
        context.RefreshTokens.RemoveRange(oldTokens);

        await context.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiryDate = expiry
        });

        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> ValidateAsync(string token)
    {
        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked && t.ExpiryDate > DateTime.UtcNow);

        return refreshToken;
    }

    public async Task RevokeAsync(string token)
    {
        var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            await context.SaveChangesAsync();
        }
    }
}