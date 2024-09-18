import { Injectable } from '@angular/core';
import { ControlledError } from '../../core/models/controlled-error.model';
import { UncontrolledError } from '../../core/models/uncontrolled-error.model';

@Injectable({
  providedIn: 'root'
})
export class ErrorLogService {

  constructor() { }

  // Ejemplo de datos para errores controlados
  getControlledErrors(): ControlledError[] {
    return [
      {
        id: "CE001",
        timestamp: new Date("2023-05-10T14:30:00"),
        message: "Usuario no encontrado",
        source: "AuthService",
        userAction: "Intento de inicio de sesión",
        expectedBehaviour: "Mostrar mensaje de error y sugerir registro"
      },
      {
        id: "CE002",
        timestamp: new Date("2023-05-10T15:45:00"),
        message: "Sesión expirada",
        source: "SessionManager",
        userAction: "Acceso a página protegida",
        expectedBehaviour: "Redirigir a página de inicio de sesión"
      }
    ];
  }

  // Ejemplo de datos para errores no controlados
  getUncontrolledErrors(): UncontrolledError[] {
    return [
      {
        id: "UE001",
        timestamp: new Date("2023-05-10T02:10:00"),
        message: "Error de servidor interno",
        source: "API Gateway",
        stackTrace: "Error: Internal Server Error\n    at Server.handleRequest (/app/server.js:42:12)\n    at emitTwo (events.js:126:13)",
        environment: "Production"
      },
      {
        id: "UE002",
        timestamp: new Date("2023-05-10T03:20:00"),
        message: "Excepción no manejada",
        source: "DataProcessor",
        stackTrace: "TypeError: Cannot read property 'length' of undefined\n    at processData (/app/utils.js:15:41)\n    at Object.<anonymous> (/app/index.js:10:3)",
        environment: "Staging"
      }
    ];
  }
}
