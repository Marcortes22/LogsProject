import { ErrorBase } from './error-base.model';

export interface UncontrolledError extends ErrorBase {
  stackTrace: string;
  environment: string;
}
