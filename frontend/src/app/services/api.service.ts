import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Employee } from '../models/employee';
import { Cab } from '../models/cab';
import { Driver } from '../models/driver';
import { CabRequest } from '../models/cab-request';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = '/api';

  constructor(private http: HttpClient) {}

  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.baseUrl}/employees`);
  }

  getEmployee(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.baseUrl}/employees/${id}`);
  }

  createEmployee(employee: Employee): Observable<Employee> {
    return this.http.post<Employee>(`${this.baseUrl}/employees`, employee);
  }

  getCabs(): Observable<Cab[]> {
    return this.http.get<Cab[]>(`${this.baseUrl}/cabs`);
  }

  getDrivers(): Observable<Driver[]> {
    return this.http.get<Driver[]>(`${this.baseUrl}/drivers`);
  }

  getCabRequests(): Observable<CabRequest[]> {
    return this.http.get<CabRequest[]>(`${this.baseUrl}/cabrequests`);
  }

  createCabRequest(request: CabRequest): Observable<CabRequest> {
    return this.http.post<CabRequest>(`${this.baseUrl}/cabrequests`, request);
  }

  updateCabRequestStatus(id: number, status: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/cabrequests/${id}/status`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
