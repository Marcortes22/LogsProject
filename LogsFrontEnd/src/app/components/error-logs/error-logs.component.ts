import { Component } from '@angular/core';
import { ErrorLog, ErrorType } from '../../../core/models/error-log.model';
import { ErrorLogService } from '../../services/error-log.service';
import { MatTabsModule } from '@angular/material/tabs';
@Component({
  selector: 'app-error-logs',
  standalone: true,
  imports: [MatTabsModule],
  templateUrl: './error-logs.component.html',
  styleUrl: './error-logs.component.css',
})
export class ErrorLogsComponent {
  controlledErrors: ErrorLog[] = [];
  uncontrolledErrors: ErrorLog[] = [];
  activeTab: string = 'controlled'; // Por defecto, el tab activo es "controlled"

  constructor(
    private errorLogService: ErrorLogService
  ) {}

  ngOnInit(): void {
    this.errorLogService.startErrorLogHubConnection().subscribe(() => {
      this.errorLogService.receiveHubErrorLog().subscribe((errorLog: ErrorLog) => {
        if (errorLog.errorType === ErrorType.Controlled) {
          this.controlledErrors.push(errorLog);
        }
        else if (errorLog.errorType === ErrorType.Uncontrolled) {
          this.uncontrolledErrors.push(errorLog);
        } else {
          console.log('Uknown ErrorType received', errorLog);
        }
      });
    });
  }

  // Método para cambiar entre las pestañas
  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }
}
