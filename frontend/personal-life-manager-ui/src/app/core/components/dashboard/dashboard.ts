import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardModel } from '../../models/dashboard.model';
import { I18nService } from '../../services/i18n.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { Heatmap } from '../heatmap/heatmap';
import { finalize } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, Heatmap, MatProgressSpinnerModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})

export class Dashboard {
  dashboard = signal<DashboardModel | null>(null);
  loading = signal(true);

  constructor(private dashboardService: DashboardService, public i18n: I18nService, private authService: AuthService, private router: Router) { }

  ngOnInit() {
    if (!this.authService.isLoggedIn()) return;

    this.loadDashboard();
  }

  loadDashboard() {

    this.loading.set(true);

    this.dashboardService
      .getDashboard()
      .pipe(
        finalize(() => {
          this.loading.set(false);
        })
      )
      .subscribe({
        next: (res) => {
          this.dashboard.set(res);
        },

        error: (err) => {
          console.error(err);
        }
      });
  }

  reloadDashboard() {
    this.loadDashboard();
  }
}
