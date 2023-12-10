import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ErrorsModel } from '../../validation/errors-model';
import { ValidationComponent } from '../../validation/validation/validation.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ValidationComponent],
  templateUrl: './register.component.html'
})
export class RegisterComponent {

  private http = inject(HttpClient);

  errors = new ErrorsModel;

  input = {
    email: '',
    password: '',
    confirmPassword: '',
  };

  emailConfirmationSent = false;
  accountRequiresConfirmation = false;

  register(): void {
    this.http.post<any>('api/accounts/register', this.input).subscribe({
      next: _ => this.accountRequiresConfirmation = true,
      error: err => {
        console.error(err);
        this.errors.set(err);
      }
    });
  }

  resendConfirmationEmail(): void {
    this.http.post<any>('api/accounts/send-email-confirmation', {
      email: this.input.email
    }).subscribe({
      next: _ => this.emailConfirmationSent = true,
      error: err => console.error(err)
    });
  }
}
