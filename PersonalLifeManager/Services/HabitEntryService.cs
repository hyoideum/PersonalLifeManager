using AutoMapper;
using PersonalLifeManager.Constants;
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
                : dates.First() == today.AddDays(DateConstants.PreviousDay)
                    ? today.AddDays(DateConstants.PreviousDay)
                    : null;

        if (expectedDate != null)
        {
            foreach (var date in dates)
            {
                if (date == expectedDate)
                {
                    currentStreak++;
                    expectedDate = expectedDate.Value.AddDays(DateConstants.PreviousDay);
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

    public async Task<List<HabitStatisticsDto>> GetStatisticsForAllHabitsAsync(string userId, DateOnly? from, DateOnly? to, DateOnly? date)
    {
        var habits = await habitRepository.GetAllAsync(userId);
        var day = date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow;
        var dailyOverview = await GetDailyOverviewAsync(userId, DateOnly.FromDateTime(day));
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
    
    public async Task<List<CalendarHeatmapDto>> GetHeatmapAsync(string userId, DateOnly fromDate, DateOnly toDate)
    {
        if (fromDate > toDate)
            throw new FromToDateException();
        
        var to = toDate ==  DateOnly.MinValue ? DateOnly.FromDateTime(DateTime.UtcNow) : toDate;
        var from = fromDate == DateOnly.MinValue ? to.AddDays(DateConstants.HeatmapPeriodDays) : fromDate;

        var data = await repository.GetHeatmapAsync(userId, from, to);
        
        var totalHabits = await habitRepository.CountActiveAsync(userId);

        // var lookup = data.ToDictionary(d => d.Date, d => d.Count);

        var result = new List<CalendarHeatmapDto>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            var existing = data.FirstOrDefault(d => d.Date == date);
            
            result.Add(new CalendarHeatmapDto
            {
                Date = date,
                Count = existing?.Count ?? 0,
                Total = totalHabits
            });
        }

        return result;
    }

    public async Task<(List<HabitStatsDto>? Best, List<HabitStatsDto>? Worst)> GetBestAndWorstHabitAsync(string userId,
        DateOnly from, DateOnly to)
    {
        if (from > to)
            throw new FromToDateException();
        
        var stats = await GetHabitStatsAdvancedAsync(userId, from, to);

        var best = stats
            .OrderByDescending(h => h.CompletionRate)
            .ThenByDescending(h => h.CurrentStreak)
            .Take(3)
            .ToList();

        var worst = stats
            .OrderBy(h => h.CompletionRate)
            .ThenBy(h => h.CurrentStreak)
            .Take(3)
            .ToList();

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
                current = current.AddDays(DateConstants.PreviousDay);
            }
            else if (date < current)
            {
                break;
            }
        }

        return streak;
    }

    public async Task<int> GetLongestStreakAsync(string userId)
    {
        var dates =
            await repository.GetCompletedDatesAsync(userId);

        var sortedDates = dates
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        int longest = 1;
        int current = 1;

        for (int i = 1; i < sortedDates.Count; i++)
        {
            var previous = sortedDates[i - 1];
            var currentDate = sortedDates[i];

            if (currentDate.DayNumber - previous.DayNumber == 1)
            {
                current++;

                if (current > longest)
                    longest = current;
            }
            else
            {
                current = 1;
            }
        }

        return longest;
    }

    public async Task<WeeklyAnalyticsDto> GetWeeklyAnalyticsAsync(string userId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var currentWeekFrom = today.AddDays(DateConstants.CurrentWeekStart);

        var previousWeekFrom = today.AddDays(DateConstants.PreviousWeekStart);

        var previousWeekTo = today.AddDays(DateConstants.WeeklyPeriodDays);

        var currentWeekEntries =
            await repository.GetEntriesForUserAsync(
                userId,
                currentWeekFrom,
                today);

        var previousWeekEntries =
            await repository.GetEntriesForUserAsync(
                userId,
                previousWeekFrom,
                previousWeekTo);

        var activeHabits =
            await repository.GetActiveHabitsCountAsync(userId);

        var currentRate =
            CalculateRate(currentWeekEntries, activeHabits, 7);

        var previousRate =
            CalculateRate(previousWeekEntries, activeHabits, 7);

        return new WeeklyAnalyticsDto
        {
            CompletionRate = currentRate,

            PreviousWeekCompletionRate = previousRate,

            Trend = Math.Round(currentRate - previousRate, 2),

            BestDay = GetBestDay(currentWeekEntries),

            CompletedHabits =
                currentWeekEntries.Count(e => !e.IsDeleted),

            TotalHabits =
                activeHabits * 7
        };
    }

    public async Task<ConsistencyDto?> GetMostConsistentHabitAsync(string userId)
    {
        var habits =
            await repository.GetHabitsWithEntriesAsync(userId);

        if (!habits.Any())
            return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var result = habits
            .Select(h =>
            {
                var created =
                    DateOnly.FromDateTime(h.CreatedAt);

                var totalDays =
                    today.DayNumber - created.DayNumber + 1;

                var completedDays =
                    h.Entries.Count(e => !e.IsDeleted);

                var rate =
                    totalDays == 0
                        ? 0
                        : (double)completedDays / totalDays * 100;

                return new ConsistencyDto
                {
                    HabitName = h.Name,

                    CompletedDays = completedDays,

                    TotalDays = totalDays,

                    ConsistencyRate = Math.Round(rate, 2)
                };
            })
            .OrderByDescending(x => x.ConsistencyRate)
            .ThenByDescending(x => x.CompletedDays)
            .FirstOrDefault();

        return result;
    }

    private async Task<List<HabitStatsDto>> GetHabitStatsAdvancedAsync(
        string userId,
        DateOnly from,
        DateOnly to)
    {
        var habits = await habitRepository.GetAllAsync(userId);

        var totalDays = (to.ToDateTime(TimeOnly.MinValue) - from.ToDateTime(TimeOnly.MinValue)).Days + 1;

        var result = new List<HabitStatsDto>();

        foreach (var habit in habits)
        {
            var completedDates = await repository.GetCompletedDatesAsync(habit.Id, userId, from, to);

            var completedCount = completedDates.Count;

            var completionRate = totalDays == 0
                ? 0
                : (double)completedCount / totalDays * 100;

            var streak = await GetStreakAsync(habit.Id, userId);

            result.Add(new HabitStatsDto
            {
                HabitId = habit.Id,
                HabitName = habit.Name,
                CompletedCount = completedCount,
                TotalDays = totalDays,
                CompletionRate = completionRate,
                CurrentStreak = streak.CurrentStreak
            });
        }

        return result;
    }
    
    private double CalculateRate(
        List<HabitEntry> entries,
        int activeHabits,
        int days)
    {
        if (activeHabits == 0 || days == 0)
            return 0;

        var completed = entries.Count(e => !e.IsDeleted);

        var totalPossible = activeHabits * days;

        return Math.Round(
            (double)completed / totalPossible * 100,
            2);
    }
    
    private string GetBestDay(List<HabitEntry> entries)
    {
        if (!entries.Any())
            return "N/A";

        var bestDay = entries
            .Where(e => !e.IsDeleted)
            .GroupBy(e => e.Date)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (bestDay == null)
            return "N/A";

        return bestDay.Key.DayOfWeek.ToString();
    }
    
    private string GetBestDay(List<HabitEntry> entries, int activeHabits)
    {
        if (!entries.Any() || activeHabits == 0)
            return "N/A";

        var grouped = entries
            .GroupBy(e => e.Date)
            .Select(g => new
            {
                Date = g.Key,

                Completed = g.Count(e => !e.IsDeleted),

                Rate = (double)g.Count(e => !e.IsDeleted) / activeHabits
            })
            .OrderByDescending(x => x.Rate)
            .ThenByDescending(x => x.Completed)
            .FirstOrDefault();

        return grouped?.Date.DayOfWeek.ToString() ?? "N/A";
    }
}