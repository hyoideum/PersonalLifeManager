using AutoMapper;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Mappings;

public class HabitEntryProfile : Profile
{
    public HabitEntryProfile()
    {
        CreateMap<CreateHabitEntryDto, HabitEntry>();
        CreateMap<HabitEntry, HabitEntryDto>().ForMember(dest  => dest.Name, opt
            => opt.MapFrom(src => src.Habit.Name));
        CreateMap<Habit, DailyHabitOverviewDto>()
            .ForMember(dest => dest.HabitId,
                opt => opt.MapFrom(src => src.Id))

            .ForMember(dest => dest.HabitName,
                opt => opt.MapFrom(src => src.Name))

            .ForMember(dest => dest.IsCompleted,
                opt => opt.MapFrom(src => src.Entries.Any()))

            .ForMember(dest => dest.Note,
                opt => opt.MapFrom(src =>
                    src.Entries
                        .Select(e => e.Note)
                        .FirstOrDefault()));
    }
}