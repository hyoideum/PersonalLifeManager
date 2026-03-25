import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardModel } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = environment.apiUrl + "/dashboard";

  constructor(private http: HttpClient) {}

  getDashboard(from?: string, to?: string): Observable<DashboardModel> {
    let params: any = {};
    if (from) params.from = from;
    if (to) params.to = to;
    return this.http.get<DashboardModel>(`${this.apiUrl}/dashboard`, { params });
  }
}