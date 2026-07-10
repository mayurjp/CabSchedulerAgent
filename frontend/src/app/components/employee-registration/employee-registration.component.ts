import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Employee } from '../../models/employee';

@Component({
  selector: 'app-employee-registration',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-registration.component.html',
  styleUrl: './employee-registration.component.css'
})
export class EmployeeRegistrationComponent implements OnInit {
  employees: Employee[] = [];
  employee: Employee = { id: 0, name: '', email: '', department: '', pickupLat: 0, pickupLng: 0 };
  message = '';

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.apiService.getEmployees().subscribe({
      next: (data) => this.employees = data,
      error: () => this.message = 'Failed to load employees.'
    });
  }

  register(): void {
    this.apiService.createEmployee(this.employee).subscribe({
      next: () => {
        this.message = 'Employee registered successfully!';
        this.employee = { id: 0, name: '', email: '', department: '', pickupLat: 0, pickupLng: 0 };
        this.loadEmployees();
      },
      error: () => this.message = 'Failed to register employee.'
    });
  }
}
