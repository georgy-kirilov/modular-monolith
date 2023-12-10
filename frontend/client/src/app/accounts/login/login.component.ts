import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';
import { ValidationComponent } from '../../validation/validation/validation.component';
import { ErrorsModel } from '../../validation/errors-model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, ValidationComponent],
  templateUrl: './login.component.html'
})
export class LoginComponent {

  private authService = inject(AuthService);
  private http = inject(HttpClient);

  errors = new ErrorsModel;

  input = {
    email: '',
    password: '',
    storeJwtInCookie: true
  };

  login(): void {
    this.http.post<any>('api/accounts/login', this.input).subscribe({
      next: res => this.authService.login(res.lifetimeInSeconds),
      error: err => this.errors.set(err)
    });
  }
}
