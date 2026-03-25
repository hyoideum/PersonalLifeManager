import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardModel } from '../../models/dashboard.model';
import { I18nService } from '../../services/i18n.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})

export class Dashboard {
  dashboard = signal<DashboardModel | null>(null);

  constructor(private dashboardService: DashboardService, public i18n: I18nService, private authService: AuthService, private router: Router) { }

  ngOnInit() {
    if (!this.authService.isLoggedIn()) return;

    this.dashboardService.getDashboard().subscribe({
      next: data => this.dashboard.set(data),
      error: err => {
        console.error(err);

        if (err.status === 401) {
          this.authService.logout();
          this.router.navigate(['auth/login']);
        }
      }
    });
  }
}
