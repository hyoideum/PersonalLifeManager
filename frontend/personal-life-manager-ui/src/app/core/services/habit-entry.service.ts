import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DailyHabit } from '../models/daily-habit.model';
import { HabitStatistics } from '../models/habit.statistics.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class HabitEntryService {
  private entryApiUrl = `${environment.apiUrl}/habitEntry`;

  constructor(private http: HttpClient) { }

  getDailyHabits(date: string) {
    return this.http.get<DailyHabit[]>(
      `${this.entryApiUrl}/daily?date=${date}`
    );
  }

  toggleHabit(habitId: number, date: string) {
    return this.http.post(
      `${this.entryApiUrl}/toggle?habitId=${habitId}&date=${date}`, {}
    );
  }

  getStatistics(): Observable<HabitStatistics[]> {
    return this.http.get<HabitStatistics[]>(`${this.entryApiUrl}/statistics/all`);
  }
}
