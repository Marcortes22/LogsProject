import { Component } from '@angular/core';
import { ControlledError } from '../../../core/models/controlled-error.model';
import { UncontrolledError } from '../../../core/models/uncontrolled-error.model';
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
  controlledErrors: ControlledError[] = [];
  uncontrolledErrors: UncontrolledError[] = [];
  activeTab: string = 'controlled'; // Por defecto, el tab activo es "controlled"

  constructor(
    private errorLogService: ErrorLogService
  ) {}

  ngOnInit(): void {
    // Cargar los datos al iniciar el componente
    this.controlledErrors = this.errorLogService.getControlledErrors();
    this.uncontrolledErrors = this.errorLogService.getUncontrolledErrors();
  }

  // Método para cambiar entre las pestañas
  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }
}
