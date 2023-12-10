export class ErrorsModel {
  errors: ValidationError[] = [];

  set(error: any): void {
    this.errors = [];

    for (const [key, value] of Object.entries(error.error.errors)) {
      const values: any[] = value as any[];

      const errors = values.map((err: any) => {
        return { field: key, code: err.code, message: err.message } as ValidationError;
      });

      this.errors.push(...errors);
    }    
  }
}

export interface ValidationError {
  field: string;
  code: string;
  message: string;
}
