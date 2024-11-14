import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {jwtDecode} from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7120/Log';

  constructor(private http: HttpClient) {}

  requestToken(username: string, password: string): Observable<string> {
    const body = {
      UserName: username,
      Password: password
    };
    return this.http.post(`${this.apiUrl}/request-token`, body, { responseType: 'text' });
  }

  decodeToken(token: string): any {
    try {
      return jwtDecode(token);
    } catch (error) {
      console.error('Error al decodificar el token:', error);
      return null;
    }
  }
  logout(): void {
    localStorage.removeItem('token');
    console.log('Sesión cerrada');
  }
}
