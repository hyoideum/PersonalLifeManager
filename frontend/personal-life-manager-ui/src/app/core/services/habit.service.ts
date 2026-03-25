import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Habit } from '../models/habit.model';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HabitStatistics } from '../models/habit.statistics.model';

@Injectable({ providedIn: 'root' })
export class HabitService {
    private habitsApiUrl = `${environment.apiUrl}/habit`;
    private entryApiUrl = `${environment.apiUrl}/habitEntry`;

    constructor(private http: HttpClient) { }

    getHabits(): Observable<Habit[]> {
        return this.http.get<Habit[]>(`${this.habitsApiUrl}`);
    }

    getHabitById(id: number): Observable<Habit> {
        return this.http.get<Habit>(`${this.habitsApiUrl}/${id}`);
    }

    addHabit(habit: Partial<Habit>): Observable<Habit> {
        return this.http.post<Habit>(`${this.habitsApiUrl}`, habit);
    }

    updateHabit(id: number, habit: Partial<Habit>): Observable<Habit> {
        return this.http.put<Habit>(`${this.habitsApiUrl}/${id}`, habit);
    }

    deleteHabit(id: number): Observable<void> {
        return this.http.delete<void>(`${this.habitsApiUrl}/${id}`);
    }
}