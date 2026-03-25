export interface HabitStatistics
{
    habitId: number;
    name: string;
    description: string;
    from: string;
    to: string;
    totalDays: number;
    completedDays: number;
    completionRate: number;
    completedToday: boolean;
}