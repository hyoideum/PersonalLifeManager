using AutoMapper;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Mappings;

public class HabitProfile : Profile
{
    public HabitProfile()
    {
        CreateMap<Habit, HabitDto>().ForMember(dest => dest.CreatedAt,
            opt => opt.MapFrom(src => DateOnly.FromDateTime(src.CreatedAt)));

        CreateMap<CreateHabitDto, Habit>();

        CreateMap<UpdateHabitDto, Habit>();
    }
}