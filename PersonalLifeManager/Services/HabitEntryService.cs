using System.Runtime.InteropServices.JavaScript;
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

    public async Task<HabitStatisticsDto> GetStatisticsAsync(int habitId, string userId, DateOnly? from, DateOnly? to)
    {
        
        var habit = await habitRepository.GetByIdAsync(habitId, userId);

        if (habit == null)
            throw new HabitNotFoundException();
        
        if (from > to)
            throw new FromToDateException();

        var start = from ?? DateOnly.FromDateTime(habit.CreatedAt);
        var end = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
        
        if (start < DateOnly.FromDateTime(habit.CreatedAt))
            start = DateOnly.FromDateTime(habit.CreatedAt);

        var completedDates =
            await repository.GetCompletedDatesAsync(habitId, userId, start, end);

        var totalDays = start.DayNumber - end.DayNumber + 1;
        var completedDays = completedDates.Count;

        var completionRate =
            totalDays == 0
                ? 0
                : Math.Round((double)completedDays / totalDays * 100, 2);

        return new HabitStatisticsDto
        {
            HabitId = habitId,
            Name = habit.Name,
            From = start,
            To = end,
            TotalDays = totalDays,
            CompletedDays = completedDays,
            CompletionRate = completionRate
        };
    }

    public async Task<List<HabitStatisticsDto>> GetStatisticsForAllHabitsAsync(string userId, DateOnly? from, DateOnly? to)
    {
        var habits = await habitRepository.GetAllAsync(userId);
        var dailyOverview = await GetDailyOverviewAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
        var result = new List<HabitStatisticsDto>();
        
        var todayStatus = dailyOverview.ToDictionary(x => x.HabitId, x => x.IsCompleted);
        
        foreach (var habit in habits)
        {
            var start = from ?? DateOnly.FromDateTime(habit.CreatedAt);
            var end = to ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var createdDate = DateOnly.FromDateTime(habit.CreatedAt);

            if (start < createdDate)
                start = createdDate;

            if (start > end)
                continue;

            var completedDates =
                await repository.GetCompletedDatesAsync(habit.Id, userId, start, end);

            var totalDays = end.DayNumber - start.DayNumber + 1;
            var completedDays = completedDates.Count;

            var completionRate =
                totalDays == 0
                    ? 0
                    : Math.Round((double)completedDays / totalDays * 100, 2);
            
            var isCompletedToday = todayStatus.ContainsKey(habit.Id) && todayStatus[habit.Id];

            result.Add(new HabitStatisticsDto
            {
                HabitId = habit.Id,
                Name = habit.Name,
                Description = habit.Description,
                From = start,
                To = end,
                TotalDays = totalDays,
                CompletedDays = completedDays,
                CompletionRate = completionRate,
                CompletedToday = isCompletedToday
            });
        }

        return result;
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

    public async Task<(List<HabitStatsDto>? Best, List<HabitStatsDto>? Worst)> GetBestAndWorstHabitAsync(string userId,
        DateOnly from, DateOnly to)
    {
        if (from > to)
            throw new FromToDateException();
        
        var stats = await repository.GetHabitStatsAsync(userId, from, to);

        if (stats.Count == 0)
            return (null, null);
        
        var maxCompleted = stats.Max(h => h.CompletedCount);
        var minCompleted = stats.Min(h => h.CompletedCount);
        
        if (maxCompleted == 0)
            return (null, null);
        
        var bestHabits = stats
            .Where(h => h.CompletedCount == maxCompleted)
            .ToList();

        var worstHabits = stats
            .Where(h => h.CompletedCount == minCompleted)
            .ToList();

        return (bestHabits, worstHabits);
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