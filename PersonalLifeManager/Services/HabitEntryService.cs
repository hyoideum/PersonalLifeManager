using AutoMapper;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Exceptions;
using PersonalLifeManager.Models;
using PersonalLifeManager.Repositories;

namespace PersonalLifeManager.Services;

public class HabitEntryService(IHabitEntryRepository repository, IHabitRepository habitRepository, IMapper mapper) : IHabitEntryService
{
    public async Task<HabitEntryDto> AddEntryAsync(CreateHabitEntryDto dto, string userId)
    {
        var existForUser = await habitRepository.ExistsForUserAsync(dto.HabitId, userId);

        if (!existForUser)
            throw new HabitNotFoundException();
        
        var exists = await repository.ExistsAsync(userId, dto.HabitId, dto.Date);
        
        if(exists)
            throw new DuplicateEntryException();
        
        var entry = mapper.Map<HabitEntry>(dto);
        entry.UserId = userId;
        
        await repository.AddAsync(entry);
        await repository.SaveChangesAsync();
        
        var entryWithHabit = await repository.GetByIdAsync(entry.Id, userId);

        return mapper.Map<HabitEntryDto>(entryWithHabit);
    }

    public async Task<List<HabitEntryDto>> GetEntriesAsync(string userId, DateOnly from, DateOnly to)
    {
        var entries = await repository.GetByUserAsync(userId, from, to);
        return mapper.Map<List<HabitEntryDto>>(entries);
    }

    public async Task<HabitEntryDto?> GetByIdAsync(int id, string userId)
    {
        var entry = await repository.GetByIdAsync(id, userId);
        
        return entry == null ? null : mapper.Map<HabitEntryDto>(entry);
    }

    public async Task DeleteEntryAsync(int id, string userId)
    {
        var entry = await repository.GetByIdAsync(id, userId);
        if (entry == null)
            throw new HabitNotFoundException();

        await repository.DeleteAsync(entry);
    }

    public async Task<HabitEntryDto> ToggleAsync(int habitId, DateOnly date, string userId)
    {
        var existForUser = await habitRepository.ExistsForUserAsync(habitId, userId);

        if (!existForUser)
            throw new HabitNotFoundException();
        
        var entry = await repository.GetByHabitAndDateAsync(userId, habitId, date);

        if (entry == null)
        {
            entry = new HabitEntry
            {
                HabitId = habitId,
                UserId = userId,
                Date = date,
                IsDeleted = false
            };

            await repository.AddAsync(entry);
        } else if (entry.IsDeleted)
        {
            entry.IsDeleted = false;
        }
        else
        {
            entry.IsDeleted = true;
        }
        
        await repository.SaveChangesAsync();
        var entryWithHabit = await repository.GetByIdAsync(entry.Id, userId);
        
        return mapper.Map<HabitEntryDto>(entryWithHabit);
    }

    public async Task<List<DailyHabitOverviewDto>> GetDailyOverviewAsync(string userId, DateOnly date)
    {
        return await repository.GetDailyOverviewAsync(userId, date);
    }

    public async Task<HabitStreakDto> GetStreakAsync(int habitId, string userId)
    {
        var dates = await repository.GetCompletedForHabitAsync(habitId, userId);

        if (!dates.Any())
        {
            return new HabitStreakDto
            {
                HabitId = habitId,
                CurrentStreak = 0,
                LongestStreak = 0
            };
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        int currentStreak = 0;

        DateOnly? expectedDate =
            dates.First() == today
                ? today
                : dates.First() == today.AddDays(-1)
                    ? today.AddDays(-1)
                    : null;

        if (expectedDate != null)
        {
            foreach (var date in dates)
            {
                if (date == expectedDate)
                {
                    currentStreak++;
                    expectedDate = expectedDate.Value.AddDays(-1);
                }
                else
                {
                    break;
                }
            }
        }

        int longestStreak = 1;
        int temp = 1;

        var asc = dates.OrderBy(d => d).ToList();

        for (int i = 1; i < asc.Count; i++)
        {
            if (asc[i] == asc[i - 1].AddDays(1))
            {
                temp++;
                longestStreak = Math.Max(longestStreak, temp);
            }
            else
            {
                temp = 1;
            }
        }

        return new HabitStreakDto
        {
            HabitId = habitId,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak
        };
    }

    public async Task<HabitStatisticsDto> GetStatisticsAsync(int habitId, string userId, DateOnly from, DateOnly to)
    {
        if (from > to)
            throw new FromToDateException();

        var completedDates =
            await repository.GetCompletedDatesAsync(habitId, userId, from, to);

        var totalDays = to.DayNumber - from.DayNumber + 1;
        var completedDays = completedDates.Count;

        var completionRate =
            totalDays == 0
                ? 0
                : Math.Round((double)completedDays / totalDays * 100, 2);

        return new HabitStatisticsDto
        {
            HabitId = habitId,
            From = from,
            To = to,
            TotalDays = totalDays,
            CompletedDays = completedDays,
            CompletionRate = completionRate
        };
    }
    
    public async Task<GlobalStatisticsDto> GetGlobalStatisticsAsync(string userId, DateOnly from, DateOnly to)
    {
        if (from > to)
            throw new FromToDateException();

        var totalDays = to.DayNumber - from.DayNumber + 1;

        var totalHabits =
            await repository.CountActiveHabitsAsync(userId);

        var totalCompletions =
            await repository.CountEntriesAsync(userId, from, to);

        var averagePerDay =
            totalDays == 0
                ? 0
                : Math.Round((double)totalCompletions / totalDays, 2);

        var maxPossibleCompletions = totalHabits * totalDays;

        var completionRate =
            maxPossibleCompletions == 0
                ? 0
                : Math.Round(
                    (double)totalCompletions / maxPossibleCompletions * 100, 2);

        return new GlobalStatisticsDto
        {
            From = from,
            To = to,
            TotalDays = totalDays,
            TotalHabits = totalHabits,
            TotalCompletions = totalCompletions,
            AveragePerDay = averagePerDay,
            CompletionRate = completionRate
        };
    }
    
    public async Task<List<CalendarHeatmapDto>> GetHeatmapAsync(string userId, DateOnly from, DateOnly to)
    {
        if (from > to)
            throw new FromToDateException();

        var data = await repository.GetHeatmapAsync(userId, from, to);

        var lookup = data.ToDictionary(d => d.Date, d => d.Count);

        var result = new List<CalendarHeatmapDto>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            result.Add(new CalendarHeatmapDto
            {
                Date = date,
                Count = lookup.GetValueOrDefault(date, 0)
            });
        }

        return result;
    }

    public async Task<(HabitStatsDto? Best, HabitStatsDto? Worst)> GetBestAndWorstHabitAsync(string userId,
        DateOnly from, DateOnly to)
    {
        if (from > to)
            throw new FromToDateException();
        
        var stats = await repository.GetHabitStatsAsync(userId, from, to);

        if (stats.Count == 0)
            return (null, null);

        var best = stats
            .OrderByDescending(h => h.CompletedCount)
            .First();

        var worst = stats
            .OrderBy(h => h.CompletedCount)
            .First();

        return (best, worst);
    }

    public async Task<int> CountCompletedForDayAsync(string userId, DateOnly date)
    {
        return await repository
            .CountCompletedForDayAsync(userId, date);
    }

    public async Task<int> GetCurrentStreakAsync(string userId, DateOnly today)
    {
        var dates =
            await repository.GetCompletedDatesAsync(userId);

        var streak = 0;
        var current = today;

        foreach (var date in dates)
        {
            if (date == current)
            {
                streak++;
                current = current.AddDays(-1);
            }
            else if (date < current)
            {
                break;
            }
        }

        return streak;
    }
}