import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { HeatmapService } from '../../services/heatmap.service';
import { CommonModule } from '@angular/common';
import { HabitEntryService } from '../../services/habit-entry.service';
import { BehaviorSubject } from 'rxjs';
import { Output, EventEmitter } from '@angular/core';
import { I18nService } from '../../services/i18n.service';

export interface HeatmapDay {
  date: string;
  count: number;
  total: number;
}

export interface DayStats {
  habitId: number;
  habitName: string;
  isCompleted: boolean;
  note?: string;
}

@Component({
  selector: 'app-heatmap',
  imports: [CommonModule],
  templateUrl: './heatmap.html',
  styleUrl: './heatmap.css',
})
export class Heatmap implements OnChanges {
  @Input() data: HeatmapDay[] = [];
  heatmap: HeatmapDay[] = [];
  selectedDay: HeatmapDay | null = null;
  months: { name: string; column: number }[] = [];
  dayDetails: DayStats[] = [];
  private heatmapSubject = new BehaviorSubject<HeatmapDay[]>([]);
  heatmap$ = this.heatmapSubject.asObservable();
  @Output() dashboardRefresh = new EventEmitter<void>();

  constructor(private service: HeatmapService, private entryService: HabitEntryService, public i18n: I18nService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data'] && this.data) {
      const mapped = this.mapToGrid(this.data);

      this.heatmapSubject.next(mapped);
      this.generateMonths(mapped);
    }
  }

  get heatmapValue(): HeatmapDay[] {
    return this.heatmapSubject.value;
  }

  mapToGrid(data: HeatmapDay[]): HeatmapDay[] {
    if (!data.length) return [];

    const sorted = [...data].sort((a, b) =>
      a.date.localeCompare(b.date)
    );

    const firstDate =
      new Date(sorted[0].date + 'T00:00:00');

    const lastDate =
      new Date(sorted[sorted.length - 1].date + 'T00:00:00');

    const start =
      this.getStartOfWeek(firstDate);

    const result: HeatmapDay[] = [];

    const current = new Date(start);

    while (current <= lastDate) {

      const dateStr =
        this.toLocalDateString(current);

      const existing =
        sorted.find(d => d.date === dateStr);

      result.push({
        date: dateStr,
        count: existing?.count ?? 0,
        total: existing?.total ?? 0
      });

      current.setDate(current.getDate() + 1);
    }

    return result;
  }

  generateMonths(data: HeatmapDay[]) {
    this.months = [];

    data.forEach((day, index) => {
      const date = new Date(day.date + 'T00:00:00');
      const month = date.toLocaleString('default', { month: 'short' });

      const column = Math.floor(index / 7);

      if (!this.months.find(m => m.name === month)) {
        this.months.push({ name: month, column });
      }
    });
  }

  getColor(completed: number, total: number): string {
    if (total === 0) return '#ebedf0';

    const ratio = completed / total;

    if (ratio === 0) return '#ebedf0';
    if (ratio < 0.25) return '#c6e48b';
    if (ratio < 0.5) return '#7bc96f';
    if (ratio < 0.75) return '#239a3b';

    return '#196127';
  }

  getStartOfWeek(date: Date): Date {
    const d = new Date(date);
    const day = d.getDay();

    const diff = (day === 0 ? -6 : 1 - day);
    d.setDate(d.getDate() + diff);

    return d;
  }

  getTooltip(day: HeatmapDay): string {
    return `${day.date}: ${day.count}/${day.total} habits completed`;
  }

  onDayClick(day: HeatmapDay) {
    this.selectedDay = day;
    this.openDetails(day.date);
  }

  openDetails(date: string) {
    this.refreshDay(date);
  }

  prevDay() {
    if (!this.selectedDay) return;

    const date = this.parseDate(this.selectedDay.date);
    date.setDate(date.getDate() - 1);

    const newDateStr = this.formatDate(date);

    this.selectedDay.date = newDateStr;
    this.openDetails(newDateStr);
  }

  nextDay() {
    if (!this.selectedDay) return;

    const date = this.parseDate(this.selectedDay.date);
    date.setDate(date.getDate() + 1);

    const newDateStr = this.formatDate(date);

    this.selectedDay.date = newDateStr;
    this.openDetails(newDateStr);
  }

  closeModal() {
    this.selectedDay = null;
    this.dashboardRefresh.emit();
  }

  isToday(dateStr: string): boolean {
    return dateStr === this.toLocalDateString(new Date());
  }

  toggleHabit(habit: DayStats) {
    if (!this.selectedDay) return;

    const date = this.selectedDay.date;

    this.entryService.toggleHabit(habit.habitId, this.selectedDay.date)
      .subscribe({
        next: (res) => {
          this.refreshDay(this.selectedDay!.date);
        },
        error: (err) => {
          console.error('toggle failed', err);
        }
      });
  }

  refreshDay(date: string) {
    this.service.getDayDetails(date).subscribe(res => {
      this.dayDetails = res;
    });

    this.service.getHeatmap().subscribe(res => {
      const mapped = this.mapToGrid(res);
      this.heatmapSubject.next(mapped);
      this.generateMonths(mapped);
    });
  }

  private parseDate(dateStr: string): Date {
    const [year, month, day] = dateStr.split('-').map(Number);
    return new Date(year, month - 1, day);
  }

  private formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  private toLocalDateString(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1)
      .toString()
      .padStart(2, '0');

    const day = date.getDate()
      .toString()
      .padStart(2, '0');

    return `${year}-${month}-${day}`;
  }
}

