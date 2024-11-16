import { Component } from '@angular/core';
import { ErrorLogService } from '../../services/error-log.service';
import { DatePipe } from '@angular/common';
import { ErrorLog, ErrorType } from '../../../core/models/error-log.model';

@Component({
  selector: 'app-error-logs',
  templateUrl: './error-logs.component.html',
  styleUrls: ['./error-logs.component.css'],
  providers: [DatePipe], // Asegúrate de incluir el DatePipe aquí
})
export class ErrorLogsComponent {
  uncontrolledErrors: ErrorLog[] = [];
  exceptionErrors: ErrorLog[] = [];
  activeTab: string = 'uncontrolled'; // Por defecto, mostrar errores no controlados

  constructor(private errorLogService: ErrorLogService, private datePipe: DatePipe) {}

  ngOnInit(): void {
    this.errorLogService.startErrorLogHubConnection().subscribe(() => {
      this.errorLogService.receiveHubErrorLog().subscribe((errorLogs: ErrorLog[] | ErrorLog) => {
        const errors = Array.isArray(errorLogs) ? errorLogs : [errorLogs];
        errors.forEach((errorLog) => {
          errorLog.createdAt = this.formatDate(errorLog.createdAt); // Formatear fecha al recibir
          if (errorLog.errorType === ErrorType.Uncontrolled) {
            this.uncontrolledErrors.push(errorLog);
          } else if (errorLog.errorType === ErrorType.Excepcion) {
            this.exceptionErrors.push(errorLog);
          }
        });
      });
    });
  }

  // Método para formatear las fechas
  private formatDate(date: string): string {
    return this.datePipe.transform(date, 'dd/MM/yyyy HH:mm:ss') || date; // Formato deseado
  }

  // Método para cambiar entre las pestañas
  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }
}
