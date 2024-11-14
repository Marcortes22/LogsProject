import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7120/Log';

  constructor(private http: HttpClient) {}

  requestToken(username: string, password: string): Observable<any> {
    const body = {
      UserName: username,
      Password: password
    };
    return this.http.post(`${this.apiUrl}/request-token`, body, { responseType: 'text' });
  }

  decodeToken(token: string): any {
    try {
      return jwt_decode(token);
    } catch (Error) {
      return null;
    }
  }
}
function jwt_decode(token: string): any {
  throw new Error('Function not implemented.');
}

