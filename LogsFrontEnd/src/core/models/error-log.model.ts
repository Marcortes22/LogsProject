export enum ErrorType {
  Controlled = 'CONTROLLED',
  Uncontrolled = 'UNCONTROLLED',
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
