import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    MatSnackBarModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  hide = true;
  username: string = '';
  password: string = '';

  constructor(private router: Router, private authService: AuthService, private snackBar: MatSnackBar) {}

  togglePasswordVisibility(event: Event) {
    event.preventDefault();
    this.hide = !this.hide;
  }

  onLogin() {
    this.authService.requestToken(this.username, this.password).subscribe({
      next: (response) => {
        console.log('Token:', response);
        const decodedToken = this.authService.decodeToken(response);
        console.log('Decoded Token:', decodedToken);
        this.router.navigate(['/error-logs']);
      },
      error: (error) => {
        console.error('Error:', error);
       
        this.snackBar.open('Credenciales inválidas. Por favor, inténtalo de nuevo.', 'Cerrar', {
          duration: 5000,
          horizontalPosition: 'right', 
          verticalPosition: 'top',     
          panelClass: ['custom-snackbar-error'] 
        });
      },
      complete: () => {
        this.snackBar.open('Inicio de sesión exitoso.', 'Cerrar', {
          duration: 2000,
          horizontalPosition: 'right',
          verticalPosition: 'top',
          panelClass: ['custom-snackbar-success']
        });
      }
    });
  }
}
