using AutoMapper;
using PersonalLifeManager.DTOs;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Mappings;

public class HabitProfile : Profile
{
    public HabitProfile()
    {
        CreateMap<Habit, HabitDto>();

        CreateMap<CreateHabitDto, Habit>();

        CreateMap<UpdateHabitDto, Habit>();
    }
}