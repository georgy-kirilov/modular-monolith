export class ErrorsModel {
  errors: ValidationError[] = [];

  set(error: any): void {
    this.errors = error.error.map((e: any) => {
        return { field: e.propertyName, code: e.errorCode, message: e.errorMessage } as ValidationError
    })
  }
}

export interface ValidationError {
  field: string;
  code: string;
  message: string;
}