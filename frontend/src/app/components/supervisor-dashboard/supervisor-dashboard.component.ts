import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { SupervisorDashboard } from '../../models/dashboard';

@Component({
  selector: 'app-supervisor-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './supervisor-dashboard.component.html',
  styleUrl: './supervisor-dashboard.component.css'
})
export class SupervisorDashboardComponent implements OnInit {
  dashboard: SupervisorDashboard | null = null;
  message = '';
  loading = false;

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.apiService.getSupervisorDashboard().subscribe({
      next: (data) => this.dashboard = data,
      error: () => this.message = 'Failed to load dashboard.'
    });
  }

  runMorning(): void {
    this.loading = true;
    this.apiService.runMorningCycle().subscribe({
      next: (res) => {
        this.message = res.message;
        this.loading = false;
        this.loadDashboard();
      },
      error: () => {
        this.message = 'Failed to run morning cycle.';
        this.loading = false;
      }
    });
  }

  runEvening(): void {
    this.loading = true;
    this.apiService.runEveningCycle().subscribe({
      next: (res) => {
        this.message = res.message;
        this.loading = false;
        this.loadDashboard();
      },
      error: () => {
        this.message = 'Failed to run evening cycle.';
        this.loading = false;
      }
    });
  }
}
