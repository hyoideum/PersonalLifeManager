export interface HabitStats {
  habitId: number;
  habitName: string;
  completedCount: number;
}

export interface GlobalStatistics {
  from: string;
  to: string;
  totalDays: number;
  totalHabits: number;
  totalCompletions: number;
  averagePerDay: number;
  completionRate: number;
}

export interface DashboardModel {
  globalStatistics: GlobalStatistics;
  bestHabits: HabitStats[] | null;
  worstHabits: HabitStats[] | null;
  todayCompleted: number;
  totalHabits: number;
  currentStreak: number;
}