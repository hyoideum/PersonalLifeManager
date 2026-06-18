import { HeatmapDay } from "../components/heatmap/heatmap";

export interface HabitStats {
  habitId: number;
  habitName: string;
  completedCount: number;
  totalDays: number;
  completionRate: number;
  currentStreak: number;
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

export interface WeeklyAnalytics {
  completionRate: number;
  previousWeekCompletionRate: number;
  trend: number;
  bestDay: string;
}

export interface MostConsistentHabit {
  habitName: string;
  longestStreak: number;
}

export interface DashboardModel {
  globalStatistics: GlobalStatistics;
  weeklyAnalytics: WeeklyAnalytics;
  bestHabits: HabitStats[] | null;
  worstHabits: HabitStats[] | null;
  mostConsistentHabit: MostConsistentHabit;
  todayCompleted: number;
  totalHabits: number;
  currentStreak: number;
  longestStreak: number;
  heatmap: HeatmapDay[] | null;
}

export interface DailyStats {
  date: string;
  completed: number;
  total: number;
}