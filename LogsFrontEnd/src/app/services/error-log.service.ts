import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Observable } from 'rxjs';
import { ErrorLog } from '../../core/models/error-log.model';

@Injectable({
  providedIn: 'root',
})
export class ErrorLogService {
  private errorLogHubConnection: HubConnection;

  constructor() {
    this.errorLogHubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5227/hub/error-logs')
      .build();
  }

  startErrorLogHubConnection(): Observable<void> {
    return new Observable<void>((observer) => {
      this.errorLogHubConnection
        .start()
        .then(() => {
          console.log('Connection established with SignalR hub');
          observer.next();
          observer.complete();
        })
        .catch((error) => {
          console.error('Error connecting to SignalR hub:', error);
          observer.error(error);
        });
    });
  }

  receiveHubErrorLog(): Observable<ErrorLog[] | ErrorLog> {
    return new Observable<ErrorLog[] | ErrorLog>((observer) => {
      this.errorLogHubConnection.on('SendErrorLogToUser', (message: ErrorLog[] | ErrorLog) => {
        observer.next(message);
      });
    });
  }
}
