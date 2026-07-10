import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Employee } from '../models/employee';
import { Cab } from '../models/cab';
import { Driver } from '../models/driver';
import { CabRequest } from '../models/cab-request';
import { Route } from '../models/route';
import { Assignment } from '../models/assignment';
import { SupervisorDashboard } from '../models/dashboard';

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

  runMorningCycle(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/scheduler/run-morning`, {});
  }

  runEveningCycle(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/scheduler/run-evening`, {});
  }

  getRoutes(): Observable<Route[]> {
    return this.http.get<Route[]>(`${this.baseUrl}/routes`);
  }

  getAssignments(): Observable<Assignment[]> {
    return this.http.get<Assignment[]>(`${this.baseUrl}/assignments`);
  }

  getSupervisorDashboard(): Observable<SupervisorDashboard> {
    return this.http.get<SupervisorDashboard>(`${this.baseUrl}/supervisor/dashboard`);
  }

  sendChatMessage(employeeId: number, message: string): Observable<{ reply: string }> {
    return this.http.post<{ reply: string }>(`${this.baseUrl}/chat/employee/message`, { employeeId, message });
  }

  cancelRequest(requestId: number, reason: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/scheduler/cancel/${requestId}`, { reason });
  }

  sendNotification(recipient: string, channel: string, message: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/notifications/send`, { recipient, channel, message });
  }

  getFleetSummary(): Observable<{ summary: string }> {
    return this.http.get<{ summary: string }>(`${this.baseUrl}/notifications/fleet-summary`);
  }
}
