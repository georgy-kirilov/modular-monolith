import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ErrorsModel, ValidationError } from '../errors-model';

@Component({
  selector: 'validation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './validation.component.html'
})
export class ValidationComponent {

  @Input() errors = new ErrorsModel;
  @Input() for: string = '';

  getErrors(): ValidationError[] {
    const lowercaseFor = this.for.toLocaleLowerCase();
    return this.errors.errors.filter(err => err.field.toLowerCase() == lowercaseFor);
  }

}
