using Microsoft.EntityFrameworkCore;
using PersonalLifeManager.Data;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Repositories;

public class HabitEntryRepository(AppDbContext context) : Repository<HabitEntry>(context), IHabitEntryRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<HabitEntry>> GetByUserAsync(string userId, DateOnly? from = null, DateOnly? to = null)
    {
        var entries = DbSet.Where(e => e.UserId == userId && !e.IsDeleted);

        if (from.HasValue && from.Value > DateOnly.MinValue)
        {
            entries = entries.Where(e => e.Date >= to);
        }

        if (to.HasValue && to.Value > DateOnly.MinValue)
        {
            entries = entries.Where(e => e.Date <= to);
        }

        return await entries.Include(e => e.Habit).ToListAsync();
    }

    public async Task<HabitEntry?> GetByIdAsync(int id, string userId)
    {
        return await _context.HabitEntries
            .Include(e => e.Habit)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && !e.IsDeleted);
    }

    public async Task<bool> ExistsAsync(string userId, int habitId, DateOnly date)
    {
        return await DbSet.AnyAsync(e =>
            e.UserId == userId &&
            e.HabitId == habitId &&
            e.Date == date &&
            !e.IsDeleted);
    }

    public async Task<HabitEntry?> GetByHabitAndDateAsync(string userId, int habitId, DateOnly date)
    {
        return await _context.HabitEntries
            .Include(e => e.Habit)
            .FirstOrDefaultAsync(e =>
                e.UserId == userId &&
                e.HabitId == habitId &&
                e.Date == date);
    }

    public async Task<List<DailyHabitOverviewDto>> GetDailyOverviewAsync(string userId, DateOnly date)
    {
        return await _context.Habits
            .Where(h => h.UserId == userId && !h.IsDeleted )
               // && h.Entries.Any(e => e.Date == date)
            .Select(h => new DailyHabitOverviewDto
            {
                HabitId = h.Id,
                HabitName = h.Name,

                IsCompleted = h.Entries.Any(e =>
                    !e.IsDeleted &&
                    e.Date == date
                ),

                Note = h.Entries
                    .Where(e => !e.IsDeleted && e.Date == date)
                    .Select(e => e.Note)
                    .FirstOrDefault()
            })
            .OrderBy(h => h.HabitId)
            .ToListAsync();
    }

    public async Task<List<DateOnly>> GetCompletedForHabitAsync(int habitId, string userId)
    {
        return await _context.HabitEntries
            .Where(e =>
                e.HabitId == habitId &&
                e.UserId == userId &&
                !e.IsDeleted)
            .Select(e => e.Date)
            .OrderByDescending(d => d)
            .ToListAsync();
    }

    public async Task<List<DateOnly>> GetCompletedDatesAsync(int habitId, string userId, DateOnly from, DateOnly to)
    {
        return await _context.HabitEntries
            .Where(e =>
                e.HabitId == habitId &&
                e.UserId == userId &&
                !e.IsDeleted &&
                e.Date >= from &&
                e.Date <= to)
            .Select(e => e.Date)
            .ToListAsync();
    }
    
    public async Task<int> CountActiveHabitsAsync(string userId)
    {
        return await _context.Habits.CountAsync(h => h.UserId == userId && !h.IsDeleted);
    }
    
    public async Task<int> CountEntriesAsync(string userId, DateOnly from, DateOnly to)
    {
        return await _context.HabitEntries
            .CountAsync(e =>
                e.UserId == userId &&
                !e.IsDeleted &&
                e.Date >= from &&
                e.Date <= to);
    }

    public async Task<List<CalendarHeatmapDto>> GetHeatmapAsync(
        string userId,
        DateOnly from,
        DateOnly to)
    {
        return await _context.HabitEntries
            .Where(e =>
                e.UserId == userId &&
                !e.IsDeleted &&
                e.Date >= from &&
                e.Date <= to)
            .GroupBy(e => e.Date)
            .Select(g => new CalendarHeatmapDto
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync();
    }
    
    public async Task<List<HabitStatsDto>> GetHabitStatsAsync(string userId, DateOnly from, DateOnly to)
    {
        return await _context.Habits
            .Where(h => h.UserId == userId && !h.IsDeleted)
            .Select(h => new HabitStatsDto
            {
                HabitId = h.Id,
                HabitName = h.Name,
                CompletedCount = h.Entries
                    .Count(e =>
                        !e.IsDeleted &&
                        e.Date >= from &&
                        e.Date <= to)
            })
            .ToListAsync();
    }

    public async Task<int> CountCompletedForDayAsync(string userId, DateOnly date)
    {
        return await _context.HabitEntries
            .CountAsync(e =>
                e.UserId == userId &&
                !e.IsDeleted &&
                e.Date == date);
    }

    public async Task<List<DateOnly>> GetCompletedDatesAsync(string userId)
    {
        return await _context.HabitEntries
            .Where(e =>
                e.UserId == userId &&
                !e.IsDeleted)
            .Select(e => e.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync();
    }
}