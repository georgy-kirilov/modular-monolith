import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../common/auth/auth.service';
import { ValidationComponent } from '../../validation/validation/validation.component';
import { ErrorsModel } from '../../validation/errors-model';
import { LoginRequest, LoginResponse } from './login.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, ValidationComponent],
  templateUrl: './login.component.html'
})
export class LoginComponent {

  private authService = inject(AuthService);
  private httpClient = inject(HttpClient);

  hidePassword = true;

  errors = new ErrorsModel;
  input = new LoginRequest;

  login(): void {
    this.httpClient.post<LoginResponse>('api/accounts/login', this.input).subscribe({
      next: res => this.authService.login(res.lifetimeInSeconds),
      error: err => this.errors.set(err)
    });
  }

  togglePassword(): void {
    this.hidePassword = !this.hidePassword;
  }
}
