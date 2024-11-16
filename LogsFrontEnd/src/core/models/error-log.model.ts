export enum ErrorType {
  Controlled = 'CONTROLLED',
  Uncontrolled = 'UNCONTROLLED',
  Excepcion = 'Excepcion', 
}

export interface ErrorLog {
  id: string;
  message: string;
  createdAt: string;
  errorType: ErrorType;
  code: string;
  retryCount: number;
  isRetriable: boolean;
}
