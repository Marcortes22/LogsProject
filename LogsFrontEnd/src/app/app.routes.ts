import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { ErrorLogsComponent } from './components/error-logs/error-logs.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'error-logs', component: ErrorLogsComponent }
];
