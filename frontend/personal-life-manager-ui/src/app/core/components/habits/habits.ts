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
import { I18nService } from '../../services/i18n.service';

@Component({
  selector: 'app-habits',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatProgressBar, MatCheckbox],
  templateUrl: './habits.html',
})
export class Habits {
  habitsWithStatistics = signal<HabitStatistics[]>([]);
  currentDate: string = new Date().toISOString().split('T')[0];
  habits: Habit[] = [];

  constructor(private habitsService: HabitService, private dialog: MatDialog, private habitEntryService: HabitEntryService,
    public i18n: I18nService) { }

  ngOnInit() {
    this.currentDate = new Date().toISOString().split('T')[0];
    this.loadStatistics();
  }

  loadStatistics() {
    this.habitEntryService.getStatistics(this.currentDate).subscribe(data => {
      this.habitsWithStatistics.set(data);
    });
  }

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
    this.habitEntryService
      .toggleHabit(habit.habitId, this.currentDate)
      .subscribe({
        next: () => {
          this.loadStatistics();
        },
        error: err => console.error(err)
      });
  }

  prevDay() {
    const d = new Date(this.currentDate);
    d.setDate(d.getDate() - 1);
    this.currentDate = d.toISOString().split('T')[0];

    this.loadStatistics();
  }

  nextDay() {
    const today = new Date().toISOString().split('T')[0];

    if (this.currentDate >= today) return;

    const d = new Date(this.currentDate);
    d.setDate(d.getDate() + 1);
    this.currentDate = d.toISOString().split('T')[0];

    this.loadStatistics();
  }
}