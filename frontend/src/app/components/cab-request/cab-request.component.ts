import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Employee } from '../../models/employee';
import { CabRequest } from '../../models/cab-request';

@Component({
  selector: 'app-cab-request',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cab-request.component.html',
  styleUrl: './cab-request.component.css'
})
export class CabRequestComponent implements OnInit {
  employees: Employee[] = [];
  requests: CabRequest[] = [];
  cabRequest: CabRequest = {
    id: 0,
    employeeId: 0,
    pickupLocation: '',
    dropoffLocation: '',
    requestedTime: '',
    status: ''
  };
  message = '';

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.apiService.getEmployees().subscribe({
      next: (data) => this.employees = data,
      error: () => this.message = 'Failed to load employees.'
    });
    this.loadRequests();
  }

  loadRequests(): void {
    this.apiService.getCabRequests().subscribe({
      next: (data) => this.requests = data,
      error: () => this.message = 'Failed to load requests.'
    });
  }

  submitRequest(): void {
    this.apiService.createCabRequest(this.cabRequest).subscribe({
      next: () => {
        this.message = 'Cab request submitted successfully!';
        this.cabRequest = { id: 0, employeeId: 0, pickupLocation: '', dropoffLocation: '', requestedTime: '', status: '' };
        this.loadRequests();
      },
      error: () => this.message = 'Failed to submit cab request.'
    });
  }
}
