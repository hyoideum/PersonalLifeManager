import { Injectable, signal } from '@angular/core';
import { HabitEntryService } from '../services/habit-entry.service';
import { DailyHabit } from '../models/daily-habit.model';

@Injectable({
  providedIn: 'root'
})
export class HabitStore {

  habits = signal<DailyHabit[]>([]);

  today = new Date().toDateString();

  constructor(private service: HabitEntryService) {}

  loadTodayHabits() {

    this.service.getDailyHabits(this.today).subscribe({
      next: data => this.habits.set(data),
      error: err => console.error(err)
    });
  }

  toggleHabit(habitId: number) {
    this.service.toggleHabit(habitId, this.today).subscribe({
      next: () => this.loadTodayHabits(),
      error: err => console.error(err)
    });
  }
}