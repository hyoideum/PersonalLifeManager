import { Component, signal } from '@angular/core';
import { Habit } from '../../models/habit.model';
import { MatDialog } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { HabitService } from '../../services/habit.service';
import { HabitStatistics } from '../../models/habit.statistics.model';
import { HabitFormComponent } from './habit-form/habit-form';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatCheckbox } from '@angular/material/checkbox';
import { HabitEntryService } from '../../services/habit-entry.service';
import { HabitStore } from '../../stores/habits.store';

@Component({
  selector: 'app-habits',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatProgressBar, MatCheckbox],
  templateUrl: './habits.html',
})
export class Habits {
  habitsWithStatistics = signal<HabitStatistics[]>([]);

  constructor(private habitsService: HabitService, private dialog: MatDialog, private habitEntryService: HabitEntryService,
    private store: HabitStore) { }

  ngOnInit() {
    // this.loadHabits();
    this.loadStatistics();
  }

  // loadHabits() {
  //   this.habitsService.getHabits().subscribe(h => this.habits.set(h));
  // }

  loadStatistics() {
    this.habitEntryService.getStatistics().subscribe(data => {
      this.habitsWithStatistics.set(data);
    });
  }

  // addHabit() {
  //   const dialogRef = this.dialog.open(HabitFormComponent);
  //   dialogRef.componentInstance.save.subscribe((habit: Partial<Habit>) => {
  //     this.habitsService.addHabit(habit).subscribe(() => this.loadHabits());
  //     dialogRef.close();
  //   });
  // }

  // editHabit(habit: HabitStatistics) {
  //   const dialogRef = this.dialog.open(HabitFormComponent);
  //   dialogRef.componentInstance.habit = habit;
  //   dialogRef.componentInstance.save.subscribe((updated: Partial<Habit>) => {
  //     this.habitsService.updateHabit(habit.habitId, updated).subscribe(() => this.loadStatistics());
  //     dialogRef.close();
  //   });
  // }

  addHabit() {
    const dialogRef = this.dialog.open(HabitFormComponent, {
      width: '350px'
    });

    dialogRef.componentInstance.save.subscribe((newHabit: Partial<Habit>) => {
      this.habitsService.addHabit(newHabit)
        .subscribe(() => this.loadStatistics());

      dialogRef.close();
    });
  }

  editHabit(habit: HabitStatistics) {
    const dialogRef = this.dialog.open(HabitFormComponent, {
      data: habit
    });

    dialogRef.componentInstance.habit = habit;

    dialogRef.componentInstance.save.subscribe((updated: Partial<Habit>) => {
      this.habitsService.updateHabit(habit.habitId, updated)
        .subscribe(() => this.loadStatistics());

      dialogRef.close();
    });
  }

  deleteHabit(habit: HabitStatistics) {
    if (!confirm(`Delete habit "${habit.name}"?`)) return;
    this.habitsService.deleteHabit(habit.habitId).subscribe(() => this.loadStatistics());
  }

  toggleToday(habit: HabitStatistics) {
    const today = new Date().toISOString().split('T')[0];
    this.habitEntryService
      .toggleHabit(habit.habitId, today)
      .subscribe({
        next: () => {
          this.loadStatistics();
        },
        error: err => console.error(err)
      });
  }
}