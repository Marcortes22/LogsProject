import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { ErrorLogsComponent } from './components/error-logs/error-logs.component';
import { LayoutComponent } from './components/layout/layout.component';
import { AuthGuard } from './guards/auth.guard';
import { ForbiddenComponent } from './components/forbidden/forbidden.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: 'error-logs', component: ErrorLogsComponent, canActivate: [AuthGuard] }
    ]
  },
  { path: 'forbidden', component: ForbiddenComponent }
];
