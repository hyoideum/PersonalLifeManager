import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DayStats, HeatmapDay } from '../components/heatmap/heatmap';

@Injectable({
  providedIn: 'root',
})
export class HeatmapService {
  private apiUrl = `${environment.apiUrl}/habitEntry`;

  constructor(private http: HttpClient) { }

  getHeatmap(from?: string, to?: string) {
    let params: Record<string, string> = {};
    if (from) params['from'] = from;
    if (to) params['to'] = to;
    return this.http.get<HeatmapDay[]>(`${this.apiUrl}/statistics/heatmap`, { params });
  }

  getDayDetails(date: string) {
    return this.http.get<DayStats[]>(`${this.apiUrl}/daily?date=${date}`);
  }
}
