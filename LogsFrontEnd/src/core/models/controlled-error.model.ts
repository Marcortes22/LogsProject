import { ErrorBase } from './error-base.model';

export interface ControlledError extends ErrorBase {
  userAction: string;
  expectedBehaviour: string;
}
