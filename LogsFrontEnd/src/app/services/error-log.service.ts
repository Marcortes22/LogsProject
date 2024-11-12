import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Observable } from 'rxjs';
import { ErrorLog } from '../../core/models/error-log.model';
@Injectable({
  providedIn: 'root'
})
export class ErrorLogService {

  private errorLogHubConnection: HubConnection;

  constructor() {
    this.errorLogHubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7120/hub/error-logs')
      .build();
  }

  // // Ejemplo de datos para errores controlados
  // getControlledErrors(): ControlledError[] {
  //   return [
  //     {
  //       id: "CE001",
  //       timestamp: new Date("2023-05-10T14:30:00"),
  //       message: "Usuario no encontrado",
  //       source: "AuthService",
  //       userAction: "Intento de inicio de sesión",
  //       expectedBehaviour: "Mostrar mensaje de error y sugerir registro"
  //     },
  //     {
  //       id: "CE002",
  //       timestamp: new Date("2023-05-10T15:45:00"),
  //       message: "Sesión expirada",
  //       source: "SessionManager",
  //       userAction: "Acceso a página protegida",
  //       expectedBehaviour: "Redirigir a página de inicio de sesión"
  //     }
  //   ];
  // }

  // // Ejemplo de datos para errores no controlados
  // getUncontrolledErrors(): UncontrolledError[] {
  //   return [
  //     {
  //       id: "UE001",
  //       timestamp: new Date("2023-05-10T02:10:00"),
  //       message: "Error de servidor interno",
  //       source: "API Gateway",
  //       stackTrace: "Error: Internal Server Error\n    at Server.handleRequest (/app/server.js:42:12)\n    at emitTwo (events.js:126:13)",
  //       environment: "Production"
  //     },
  //     {
  //       id: "UE002",
  //       timestamp: new Date("2023-05-10T03:20:00"),
  //       message: "Excepción no manejada",
  //       source: "DataProcessor",
  //       stackTrace: "TypeError: Cannot read property 'length' of undefined\n    at processData (/app/utils.js:15:41)\n    at Object.<anonymous> (/app/index.js:10:3)",
  //       environment: "Staging"
  //     }
  //   ];
  // }

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

  receiveHubErrorLog(): Observable<ErrorLog> {
    return new Observable<ErrorLog>((observer) => {
      this.errorLogHubConnection.on('SendErrorLogToUser', (message: ErrorLog) => {
        observer.next(message);
      });
    });
  }
}
