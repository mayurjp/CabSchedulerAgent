import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { EmployeeRegistrationComponent } from './components/employee-registration/employee-registration.component';
import { CabRequestComponent } from './components/cab-request/cab-request.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: EmployeeRegistrationComponent },
  { path: 'request-cab', component: CabRequestComponent },
];
