using PersonalLifeManager.DTOs;

namespace PersonalLifeManager.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(string userId, DateOnly from, DateOnly to);
}