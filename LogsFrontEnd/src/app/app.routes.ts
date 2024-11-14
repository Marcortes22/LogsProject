import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { ErrorLogsComponent } from './components/error-logs/error-logs.component';
import { LayoutComponent } from './components/layout/layout.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: 'error-logs', component: ErrorLogsComponent }
    ]
  }
];